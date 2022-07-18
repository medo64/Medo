using System;
using System.Collections.Generic;

namespace BearBusMonitor;

internal class KeySelection {

    private readonly Queue<char> Letters = new(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' });
    private readonly Dictionary<char, Action> CharActionPairs = new();
    private readonly List<Tuple<char, string>> Entries = new();

    public KeySelection() { }


    public void Add(string text, Action action) {
        var ch = Letters.Dequeue();
        Add(ch, text, action);
    }

    public void Add(char ch, string text, Action action) {
        CharActionPairs.Add(ch, action);
        Entries.Add(new Tuple<char, string>(ch, text));
    }

    public bool Invoke(char ch) {
        if (CharActionPairs.TryGetValue(ch, out var action)) {
            action.Invoke();
            return true;
        }
        return false;
    }

    public IEnumerable<Tuple<char, string>> EnumerateEntries() {
        foreach (var entry in Entries) {
            yield return entry;
        }
    }

}
