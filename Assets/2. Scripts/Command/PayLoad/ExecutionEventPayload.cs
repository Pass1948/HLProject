using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ExecutionEventPayload
{
    public readonly ExecutionRecord Record;
    public ExecutionEventPayload(ExecutionRecord rec) { Record = rec; }
}
