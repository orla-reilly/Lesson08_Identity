namespace TheWorld.GameMechanics;

/// <summary>
/// All the data in the game!  
/// This is the one place where all the Areas, Items, and Creatures are stored.  
/// It is injected into every builder, so that they can add their new creations to the game data.
/// 
/// TODO: we need to add Players here too, so that we can save and load the game state eventually.
/// </summary>
public record GameData
{
    /// <summary>
    /// All the Area's in the game!
    /// </summary>
    private Dictionary<Guid, Area> Areas { get; init; } = [];

    /// <summary>
    /// All the Item's in the game!
    /// </summary>
    private Dictionary<Guid, Item> Items { get; init; } = [];

    /// <summary>
    /// All the Creature's in the game!
    /// </summary>
    private Dictionary<Guid, Creature> Creatures { get; init; } = [];

    //--------------------------------------------------------------
    //  Methods for adding and retrieving Areas, Items, and Creatures.
    // TODO:  This is a very simple implementation.  It does not handle duplicates, and it does not handle errors.  It is just a starting point.
    //--------------------------------------------------------------

    // Three methods named TryAdd.  They're Overloads:  C# picks one by the type you hand it.
    // add means CREATE a new record in the data set
    public bool TryAdd(Area area) => Areas.TryAdd(area.Id, area);
    public bool TryAdd(Item item) => Items.TryAdd(item.Id, item);
    public bool TryAdd(Creature creature) => Creatures.TryAdd(creature.Id, creature);

    //--------------------------------------------------------------
    //  Methods for retrieving Areas, Items, and Creatures by their label.
    // TODO:  This is a very simple implementation.  It does not handle errors.  It is just a starting point.
    //--------------------------------------------------------------

    // Get gets Overloaded too, now that every kind of label is its own Type.  (In Lesson 7 every key was a string, so it couldn't be.)
    // A null label finds nothing, so gameData.Get(tree)?.Examine() works even when tree might be null.
    //get is the READ action
    public Area? Get(AreaReferenceLabel? label) => label is null ? null : Areas.GetValueOrDefault(label.Id);
    public Item? Get(ItemReferenceLabel? label) => label is null ? null : Items.GetValueOrDefault(label.Id);
    public Creature? Get(CreatureReferenceLabel? label) => label is null ? null : Creatures.GetValueOrDefault(label.Id);

    public bool TryUpdate(Area area)
    {
        if (!Areas.ContainsKey(area.Id))
            return false;
        Areas[area.Id] = area;
        return true;
    }
}



/// <summary>
    /// A ReferenceLabel is a thing that has a label and a unique identifier.  It is pure data, and is not displayed ever.
    /// It is a reference to a thing in the GameData, and GameData.Get uses it to find that thing.
    ///
    /// This structure is useful for saving and loading game data, as it allows us to store a reference to a thing without storing the entire object.
    /// The Label and Id are written once, and the <T> says what kind of thing the label is for.
    /// The short names (ItemReferenceLabel, etc.) are Aliases, in GlobalIncludes.cs.
    ///
    /// This is called a Strongly Typed Key.  The Guid inside could open anything, but the label's Type says what kind of thing it opens,
    /// so the compiler won't let an ItemReferenceLabel go where an AreaReferenceLabel belongs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record ReferenceLabel<T> // angle brackets allow any type that you want attached to this reference label
    {
        public required string Label { get; init; }

        public required Guid Id { get; init; }
    };

