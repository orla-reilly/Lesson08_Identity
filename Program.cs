using TheWorld.GameMechanics;
using TheWorld.Display;

using static TheWorld.Rules.DiceRollStrategies;

// This is the one Random Number Generator for the whole program.  We inject it everywhere a roll happens.
Random rng = new();
// This is the one GameData for the whole program.  We inject it everywhere a new entity is created, so that it can be added to the game data.
GameData gameData = new();

var player = PlayerBuilder
    .FromName("Kyroki")
    .WithHealthPoints(rng, Dice.D6 with { Count = 2, Modifier = 1 })
    .WithAbilities(rng, Dice.D20, ClampRoll(strategy: Advantage))
    .WithWallet(Money.Roll(rng, Dice.D100, Default))
    .Build();

// Build hands back a ReferenceLabel:  a name for people to read, and an Id for the GameData to look up.
var tree = ItemBuilder
    .FromName("Tree")
    .WithDescription("A big tree.  It has a lot of leaves.")
    .Build(gameData);

Console.WriteLine(gameData.Get(tree)?.Examine() ?? "nothing to look at");

// A second tree, copied from the first.  Same name, same description, an identity of its own.
var tree2 = ItemBuilder //different type
    .FromItem(gameData.Get(tree)!) // TODO: That ! is a hack.  We need to do something better someday...
    .WithDescription("Another big tree.")
    .Build(gameData);

// One recipe, two goblins.  Each call to Build makes a brand-new Creature with an identity of its own.
var goblinRecipe = CreatureBuilder
    .FromName("Goblin")
    .WithExperienceReward(50);
var goblin = goblinRecipe.Build(gameData);
//crearure reference label is a type that refers to a creature "STRONGLY"
//strongly typed gives us error if we assign wrong type, instead of exploding at run time
var goblin2 = goblinRecipe.Build(gameData);

var g1 = gameData.Get(goblin)!;
var g2 = gameData.Get(goblin2)!;

var goblin3Ref = goblin! with { Label = "Goblin3" };

var g3 = gameData.Get(goblin3Ref);
//TESTING :::::
//var g3 = g1 with { };
//var g4 = g2 with { Id = Guid.NewGuid() };
//gameData.TryAdd(g3);//rejected, cant distinguish two reference labels that have the same ID
//gameData.TryAdd(g4);

//Console.WriteLine(g1 == g2);
//onsole.WriteLine(ReferenceEquals(g1, g3)); // editing value at runtime does not change that
                                            // of the other becasue they are different references

var forest = AreaBuilder
    .FromName("Forest")
    .WithDescription("A dark forest.  It has a lot of trees.")
    .WithItem(tree!)
    .WithItem(tree2!)
    .WithCreature(goblin!)
    .WithCreature(goblin2!)
    .Build(gameData);



Console.WriteLine(gameData.Get(forest)?.LookAround() ?? "nothing to look at");


// Part TWo: a cave of our own. CreatureBuilder. Buil (gaameData) calls Build() to make the creature, 
//so the creature still gets put together in one place,then files it and hands back a labe'
var wolf = CreatureBuilder
    .FromName("Wolf")
    .WithExperienceReward(50)
    .Build(gameData);
var bat = CreatureBuilder
    .FromName("Bat")
    .WithExperienceReward(10)
    .Build(gameData);
var lantern = ItemBuilder
    .FromName("Lantern")
    .WithDescription("a rusty lantern. somebody left it here, still burning")
    .Build(gameData);
var cave = AreaBuilder
    .FromName("Cave")
    .WithDescription("damp and dark. something drips, and something else squeaks")
    .WithCreature(goblinRecipe.Build(gameData)!)
    .WithCreature(goblinRecipe.Build(gameData)!)
    .WithCreature(wolf!)
    .WithCreature(bat!)
    .WithItem(lantern!)
    .Build(gameData);

var forestArea = gameData.Get(forest!);
forest = AreaBuilder.FromArea(forestArea)
    .WithNeighbor(cave!)
    .Update(gameData);

var caveArea = gameData.Get(cave)!;
cave = AreaBuilder.FromArea(caveArea)
    .WithNeighbor(forest! with {Label = "Outside"})
    .Update(gameData);

Console.WriteLine(gameData.Get(cave)?.LookAround() ?? "nothing to look at");

player.ShowSheet();
