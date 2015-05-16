namespace Server.Items
{
    public interface IShipWeapon : IEntity
    {
        int Facing { get; set; }
        bool FixedFacing { get; set; }
        BaseShipProjectile Projectile { get; set; }
        bool IsPackable { get; set; }
        bool IsDraggable { get; set; }
        void LoadWeapon(Mobile from, BaseShipProjectile projectile);

        void PlaceWeapon(Mobile from, Point3D location, Map map);

        void StoreWeapon(Mobile from);
    }

    public interface IShipProjectile
    {
        int AnimationID { get; }
        int AnimationHue { get; }
    }
}