
public class CarRoot
{
    public Car[] Property1 { get; set; }
}

public class Car : BaseItem
{
    public int revenue { get; set; }
}


public class TrackRoot
{
    public Track[] Property1 { get; set; }
}

public class Track : BaseItem
{
}
public class BaseItem {
    public string itemname { get; set; }
    public string desc { get; set; }
    public string prefab { get; set; }
    public int purchasecost { get; set; }
    public int maintcost { get; set; }
    public int quality { get; set; }
    public int maxspeed { get; set; }
    public int qtyInStock { get; set; }

    public int dangerContribution { get; set; }
    public string connections { get; set; }
    public BuildTile.TileTypes AssetType = BuildTile.TileTypes.SCENARY;

}

public class InventoryItem
{
    public string itemName;
    public int currentAvail;

}


