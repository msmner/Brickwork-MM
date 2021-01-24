namespace Brickwork.Models
{
    using System.Collections.Generic;
    public abstract class BaseSquare
    {
        public BaseSquare(List<int> startingPoint)
        {
            this.StartingPoint = startingPoint;
        }
        public List<int> StartingPoint { get; set; }
    }
}
