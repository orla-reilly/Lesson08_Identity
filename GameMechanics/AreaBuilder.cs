namespace TheWorld.GameMechanics;

public record AreaBuilder
{
    // Default values for every part of an Area, the same trick PlayerBuilder and CreatureBuilder use.
    private Guid _id;
    private string _name = "The Void";
    private string _description = string.Empty;
    private IReadOnlyList<CreatureReferenceLabel> _creatures = [];
    private IReadOnlyList<ItemReferenceLabel> _items = [];
    private IReadOnlyList<AreaReferenceLabel> _neighbors = [];

    private AreaBuilder() { }

    /// <summary>
    /// Start with a FROM method.  An area starts from the name of its archetype.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static AreaBuilder FromName(string name) => new() { _name = Helpers.ValidateString(name, "The Void") };

    /// <summary>
    /// Start with a FROM method.  An area starts from the parts of an existing area, which can then be changed.
    /// The Id isn't one of the parts:  Build gives every new Area an identity of its own.
    /// </summary>
    public static AreaBuilder FromArea(Area area) => new()
    {
        _id = area.Id,
        _name = area.Name,
        _description = area.Description,
        _creatures = area.Creatures,
        _items = area.Items,
        _neighbors = area.Neighbors
    };


    /// <summary>
    /// TODO: Start with a FROM method.  An area starts from a JSON string, which is parsed and modified.
    /// </summary>
    public static AreaBuilder FromJson(string json) =>
        throw new NotImplementedException("FromJson is not implemented yet.  It will be implemented in a future lesson.");

    /// <summary>
    /// Adds a description to the area being built.  This is optional, but recommended.
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public AreaBuilder WithDescription(string description) =>
        this with { _description = Helpers.ValidateString(description, "A place of nothingness.") };

    /// <summary>
    /// Adds a creature to the area being built.  The creature must already be in the GameData:
    /// build it with CreatureBuilder.Build(gameData), and pass in the label that comes back.
    /// </summary>
    public AreaBuilder WithCreature(CreatureReferenceLabel creature) =>
        this with { _creatures = [.. _creatures, creature] };

    public AreaBuilder WithItem(ItemReferenceLabel item) =>
        this with { _items = [.. _items, item] };

    /// <summary>
    /// Add a neighboring area by its label 
    /// </summary>
    /// <param name="neighbor"></param>
    /// <returns></returns>
    public AreaBuilder WithNeighbor(AreaReferenceLabel neighbor) =>
        this with { _neighbors = [.. _neighbors, neighbor] };

    /// <summary>
    /// Puts the area together from whatever parts have been set, and hands back the Area itself.
    /// Every call builds a brand-new Area, with an identity of its own.
    /// </summary>
    public Area Build() => new(_name)
    {
        Description = _description,
        Creatures = _creatures,
        Items = _items,
        Neighbors = _neighbors
    };

    /// <summary>
    /// Builds the area and adds it to the game data.
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns>A label for the new area, or null if it couldn't be added.</returns>
    public AreaReferenceLabel? Build(GameData gameData)
    {
        var area = Build();
        if (gameData.TryAdd(area))
            return new() { Id = area.Id, Label = area.Name };

        return null;
    }
    
    public AreaReferenceLabel? Update(GameData gameData)
    {
        var area = Build() with { Id = _id };
        if (gameData.TryUpdate(area))
            return new() { Id = _id, Label = area.Name };
        return null;
    }
}
