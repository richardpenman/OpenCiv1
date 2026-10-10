global using Xunit;

// The game engine keeps state in a single emulated CPU per game instance and some static helpers,
// so run the tests one at a time.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
