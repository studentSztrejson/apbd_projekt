
interface IHazardNotifier
{
    void NotifyHazard(string message);
}

public abstract class Container
{
    private static int counter = 1;
    public string SerialNumber { get; }
    public double MaxLoad { get; }
    public double CurrentLoad { get;  set; }
    public double OwnWeight { get; }
    public double Depth { get; }
    public double CargoWeight { get; set; }

    protected Container(string type, double maxLoad, double ownWeight, double depth)
    {
        SerialNumber = $"KON-{type}-{counter++}";
        MaxLoad = maxLoad;
        OwnWeight = ownWeight;
        Depth = depth;
        CurrentLoad = 0;
        CargoWeight = 0;
    }

    public void Load(double weight)
    {
        if (CargoWeight + OwnWeight > MaxLoad)
        {
            throw new InvalidOperationException("Za duzo wagi");
        }
        CargoWeight += weight;
    }

    public void Unload()
    {
        CargoWeight = 0;
    }
    public override string ToString() => $"{SerialNumber}: {CargoWeight}/{MaxLoad} kg (Own weight: {OwnWeight} kg, Depth: {Depth} cm)";

}

public class LuiquidContiauner : Container, IHazardNotifier
{
    public bool IsHazardous { get; }
    
    public LuiquidContiauner(double maxLoad, double ownWeight, double depth, bool isHazardous) : base("L", maxLoad, ownWeight, depth)
    {
        IsHazardous = isHazardous;
    }
    
    public new void Load(double weight)
    {
        double limit;
        if (IsHazardous == true)
        {
            limit = MaxLoad * 0.5;
        }
        else
        {
            limit = MaxLoad * 0.9;
        }
        if (CargoWeight + weight > limit)
            NotifyHazard(" przekrocznono limit zaladowania!");
        
        base.Load(weight);
    }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"Ostrzezenie dla {SerialNumber}: {message}");
    }
}

public class GasContainer : Container, IHazardNotifier
{
    private double Preassure { get;  }
    
    
    public GasContainer(string type, double maxLoad, double ownWeight, double depth, double preassure) : base(type, maxLoad, ownWeight, depth)
    {
        Preassure = preassure;
    }

    public new void Unload()
    {
        CargoWeight *= 0.05;
    }

    public new void Load(double weight)
    {
        base.Load(weight);
    }
    

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"Ostrzezenia dla {SerialNumber}: {message}");
    }
}

public class FreezeContainer : Container
{
    public String Product { get;  }
    public double Temperatue { get; }
    
    //slownik - sprawdzilem w internecie 
    private static readonly Dictionary<string, double> ProductTemperatures = new()
    {
        { "Bananas", 13.3 },
        { "Chocolate", 18.0 },
        { "Fish", 0.0 },
        { "Meat", -15.0 },
        { "Ice cream", -18.0 },
        { "Frozen Pizza", -30.0 },
        { "Cheese", -7.2 },
        { "Sausages", 5.0 },
        { "Butter", -20.5 },
        { "Eggs", 19.0 }
    };


    public FreezeContainer(string type, double maxLoad, double ownWeight, double depth, string product, double temperatue) : base(type, maxLoad, ownWeight, depth)
    {
        Product = product;
        Temperatue = temperatue;
        if (temperatue < ProductTemperatures[product])
        {
            throw new InvalidOperationException($"Za niska temperatura na pradukt {product}");
        }
        
    }

    public new void Load(double weight)
    {
        base.Load(weight);
    }
}

public class ContainerShip
{
    public string Name { get; }
    public int MaxContainers { get; }
    public double MaxWeight { get; }
    private List<Container> containers = new List<Container>();

    public ContainerShip(string name, int maxContainers, double maxWeight)
    {
        Name = name;
        MaxContainers = maxContainers;
        MaxWeight = maxWeight;
    }

    public double GetTotalWeight()
    {
        double weight = 0;
        foreach (var c in containers)
            weight += c.CargoWeight + c.OwnWeight;
        return weight;
    }

    public void LoadContainers(Container container)
    {
        if (containers.Count >= MaxContainers || GetTotalWeight() + container.CargoWeight + container.OwnWeight > MaxWeight)
            throw new InvalidOperationException("nie mozna wiecej zaladowac");
        
        containers.Add(container);
    }
    
    public void UnloadContainer(string serialNumber)
    {
        containers.RemoveAll(c => c.SerialNumber == serialNumber); // uzywam lamnbdy
    }
    public override string ToString()
    {
        return $"Ship {Name}: {containers.Count}/{MaxContainers} containers, {GetTotalWeight()}/{MaxWeight} kg";
    }
    
}

class Program
{
    static void Main()
    {
        ContainerShip ship = new ContainerShip("Titanic", 5, 50000);
        Container c1 = new LuiquidContiauner(10000, 2000, 300, true);
        Container c2 = new GasContainer("G", 1500, 250, 5,100);
        Container c3 = new FreezeContainer("F", 1800, 280, 20, "Fish", 21);
        
        c1.Load(5000);
        c2.Load(3000);
        c3.Load(4000);
        
        ship.LoadContainers(c1);
        ship.LoadContainers(c2);
        ship.LoadContainers(c3);
        
        Console.WriteLine(ship);
    }
}
