using System;
using System.Collections.Generic;

namespace TrainigSectorDataEntry.Models;

public partial class EntityImagesTableType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<EntityImage> EntityImages { get; set; } = new List<EntityImage>();
}
