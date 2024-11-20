using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Equipments;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public abstract class Monster : IMonster, ITargetable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int AggressionLevel { get; set; }
        public string MonsterType { get; set; }

        // Foreign key
        public int? EquipmentId { get; set; }

        // Navigational properties
        public virtual Equipment? Equipment { get; set; }

        protected Monster()
        {

        }

        public abstract void Attack(ITargetable target);
    }
}
