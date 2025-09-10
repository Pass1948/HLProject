using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct CommandEventPayload
{
    public readonly BaseCommand Command;
    public CommandEventPayload(BaseCommand cmd) { Command = cmd; }
}
