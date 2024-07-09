using System;
using System.Collections.Generic;

[Serializable]
public class CollectibleData {
    public bool isDestroyed;
}

[Serializable]
public class CollectiblesData {
    public List<CollectibleData> collectibles = new List<CollectibleData>();
}