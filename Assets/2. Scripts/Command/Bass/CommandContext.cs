using System.Collections;
using System.Collections.Generic;
using System;

public sealed class CommandContext
{
    public Random Rng { get; }
    public CommandContext(int seed = 0) { Rng = new Random(seed); }
}
