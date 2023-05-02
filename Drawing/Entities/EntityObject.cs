using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Entities
{
    public enum EntityType
    {
        Point,
        Line,
        Polygon
    }
    public abstract class EntityObject : ICloneable
    {
        private readonly EntityType type;
        protected bool selected;       

        public EntityObject(EntityType type)
        {
            this.type = type;
            this.selected = false;
            this.Visible = true;
        }
        public EntityType Type
        {
            get { return type; }
        }
        public bool Visible { get; set; }
        
        public bool Selected
        {
            get { return selected; }
        }
        public void Select()
        {
            selected = true;
        }
        public void UnSelect()
        {
            selected = false;
        }
        public abstract object Clone();
    }
}
