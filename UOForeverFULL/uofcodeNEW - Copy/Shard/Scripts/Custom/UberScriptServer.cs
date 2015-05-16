using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Server.Engines.XmlSpawner2;
using System.IO;
using Server.Commands;
using System.Xml;

namespace Server.Scripts.Custom
{
    // server communication based on tutorial at http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server
    public class UberScriptServer
    {
        private static bool Enabled = false;
        private static UberScriptServer server = null;
        private static int port = 3132;

        public static void Initialize()
        {
            if (Enabled)
            {
                server = new UberScriptServer();
                CommandSystem.Register("ToggleUberScriptUploads", AccessLevel.GameMaster, new CommandEventHandler(ToggleUberScriptUploads_Command));
            }
        }
        public static void ToggleUberScriptUploads_Command(CommandEventArgs e)
        {
            Enabled = !Enabled;
            if (Enabled)
            {
                e.Mobile.SendMessage("UberScript Uploads are now enabled.");
            }
            else
            {
                e.Mobile.SendMessage("UberScript Uploads are now disabled.");
            }
        }


        private TcpListener tcpListener;
        private Thread listenThread;

        public UberScriptServer()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, port);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
            Console.WriteLine("\nUberScriptServer listening on port " + port + "\n");
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            
            int fileNameNumChars = 0;
            string fileName = "";

            byte[] numBytesInFile = new byte[4];
            int numBytes = 0;
            byte[] fileContents = new byte[100000]; // files >100,000 bytes are not allowed
            string fileString = null;
            int bytesRead;
            
            byte[] outbuffer = null;

            
            bytesRead = 0;
            ASCIIEncoding encoder = new ASCIIEncoding();
            try
            {
                try
                {
                    if (!Enabled)
                    {
                        string notAcceptingMessage = "The server is currently not accepting files. Try again soon (it's likely that the staff is blocking uploads while an uberscript file is tested). If you still get this message after 10 minutes or so, PM Alan on the forums!";
                        clientStream.Write(encoder.GetBytes(notAcceptingMessage), 0, notAcceptingMessage.Length);
                        clientStream.Flush();
                        clientStream.Close();
                        tcpClient.Close();
                        return;
                    }
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(numBytesInFile, 0, 4); // read an int32 with the num of characters in the file name
                    //Console.WriteLine("Bytes read:" + bytesRead + " = " + numBytes);
                    fileNameNumChars = BitConverter.ToInt32(numBytesInFile, 0);
                    byte[] fileStringChars = new byte[fileNameNumChars];

                    bytesRead = clientStream.Read(fileStringChars, 0, fileNameNumChars);
                    fileName = encoder.GetString(fileStringChars);
                    if (fileName.Contains('/') || fileName.Contains('\\'))
                    {
                        throw new Exception("Uploaded file name cannot have any directory separators!");
                    }
                    //Console.WriteLine("Bytes read:" + bytesRead + " = " + fileName);

                    bytesRead = clientStream.Read(numBytesInFile, 0, 4); // read an int32 with the num of bytes in the file
                    
                    numBytes = BitConverter.ToInt32(numBytesInFile, 0);
                    //Console.WriteLine("Bytes read:" + bytesRead + ": " + numBytes);
                    int bufferPointer = 0;
                    while (bufferPointer < numBytes)
                    {
                        bytesRead = clientStream.Read(fileContents, bufferPointer, numBytes);
                        bufferPointer += bytesRead;
                        //Console.WriteLine("Bytes read:" + bytesRead);
                    }
                    fileString = encoder.GetString(fileContents, 0, bufferPointer);
                    //Console.WriteLine("Finished reading stream.");
                }
                catch (Exception e)
                {
                    //a socket error has occured
                    try
                    {
                        string error_msg = "Server had the following problem reading the file data:\n" + e.Message;
                        Console.WriteLine(error_msg);
                        outbuffer = encoder.GetBytes(error_msg);
                        clientStream.Write(outbuffer, 0, outbuffer.Length);
                        clientStream.Flush();
                        clientStream.Close();
                        tcpClient.Close();
                        return;
                    }
                    catch (Exception fallThroughFailed)
                    {
                        Console.WriteLine("UberScriptServer error: " + fallThroughFailed.Message + "\n" + fallThroughFailed.StackTrace);
                        return;
                    }
                }

                //file data has successfully been received
                string parseTestResult = null;
                bool success = false;
                if (fileString != null)
                {
                    parseTestResult = TestValidUberScript(fileString, out success);
                }
                else
                {
                    parseTestResult = "Error: somehow the string was null... check your input file!";
                }
                Console.WriteLine("UberScriptServer file parse result:\n  file = " + fileName + "   result = " + (success ? "SUCCESS" : "FAILURE"));
                outbuffer = encoder.GetBytes(parseTestResult + '\0');
                if (success)
                {
                    try
                    {
                        string path = ParsedScripts.UberScriptDirectory + System.IO.Path.DirectorySeparatorChar + fileName;
                        using (StreamWriter writer = new StreamWriter(path, false))
                        {
                            writer.WriteLine(fileString);
                        }
                        // reset scripts associated with that file as well as all gumps
                        ParsedScripts.ResetScripts_Command(new CommandEventArgs(null, "uberreset", "uberreset " + fileName, new string[] { fileName }));
                        ParsedGumps.GumpFileMap = new Dictionary<string, string>();
                        ParsedGumps.Gumps = new Dictionary<string, UberGumpBase>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("UberScriptServer write error: " + e.Message);
                        outbuffer = encoder.GetBytes("File was parsed, but an error occured while trying to write it to the file system:\n" + e.Message + "\n\nIf this error persists, contact Alan at alan.uoforever@gmail.com." + '\0');
                    }
                }

                clientStream.Write(outbuffer, 0, outbuffer.Length);
                clientStream.Flush();
                clientStream.Close();
                tcpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("UberScriptServer close connection error: " + e.Message);
            }
        }

        public static string TestValidUberScript(string fileChars, out bool result)
        {
            //Console.WriteLine("Testing file with " + fileChars.Length + " bytes");
            //Console.WriteLine(fileChars);
            string[] lines = fileChars.Split('\n');

            try
            {
                RootNode parsed = UberTreeParser.ParseFileContents(lines, null);
                if (parsed == null)
                {
                    throw new Exception(); // try xml parse
                }
            }
            catch (Exception e)
            {
                // now try parsing it as XML to see if it's an XML gump
                if (fileChars.Contains("<Gump") || fileChars.Contains("<gump") || fileChars.Contains("<GUMP"))
                {
                    try
                    {
                        XmlDocument parsedXml;
                        parsedXml = new XmlDocument();
                        parsedXml.LoadXml(fileChars);
                        UberGumpBase parsedGump = new UberGumpBase(parsedXml.FirstChild);
                    }
                    catch (Exception e2)
                    {
                        result = false;
                        string output = "Gump XML Parsing Error encountered (Send the xml of the file to Alan at alan.uoforever@gmail.com if you REALLY can't figure it out from the following message):\n\n" + e2.Message;
                        while (e2.InnerException != null)
                        {
                            output += "\n\t" + e2.InnerException.Message;
                            e2 = e2.InnerException;
                        }
                        return output;
                    }
                    result = true;
                    return @"======================== XML SUCCESS ======================
XML file was processed successfully! Your UberScripts can now send that gump by using
    SENDGUMP(mobile, gumpFileName)
If you are a GM on the test server, you can now do the following:
    [addatt xmlscript fileName
and then target the Item or Mobile you would like to attach the script to. e.g. if you just uploaded 'test.us' then you would do
    [addatt xmlscript test.us

If you are testing something in the onCreate trigger, and already have that script on the object you are testing it on, be sure to
    [delatt xmlscript
on the Mobile or Item and then [addatt that script again (onCreate trigger only executes when the script is first attached)
";
                }
                else
                {
                    result = false;
                    return "UberScript Parsing Error encountered (Send the text of the file to Alan at alan.uoforever@gmail.com if you REALLY can't figure it out from the following message):\n\n" + e.Message;
                }
            }
            result = true;
            return @"======================== SUCCESS ==========================
UberScript was processed successfully! If you are a GM on the test server, you can now do the following:
    [addatt xmlscript fileName
and then target the Item or Mobile you would like to attach the script to. e.g. if you just uploaded 'test.us' then you would do
    [addatt xmlscript test.us

If you are testing something in the onCreate trigger, and already have that script on the object you are testing it on, be sure to
    [delatt xmlscript
on the Mobile or Item and then [addatt that script again (onCreate trigger only executes when the script is first attached)";

        }
    }
}
