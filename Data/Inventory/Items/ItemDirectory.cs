using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDirectory : MonoBehaviour
{
    public List<ItemCategory> categories;
    public List<EPrefix> equipmentPrefixes;
    public List<ESuffix> equipmentSuffixes;
    public List<WPrefix> weaponPrefixes;
    public List<WSuffix> weaponSuffixes;
    public List<Color> rarityColors;
    public List<Color> robeColors;
    
    [field: SerializeField]
    public List<Color> weaponLights;
    public List<Sprite> weaponImages0;
    public List<Sprite> weaponImages1;
    public List<Sprite> projectileImages0;
    public List<Sprite> projectileImages1;
    public List<Sprite> armorImages;
    public List<Sprite> bootsImages;
    public List<Sprite> glovesImages;
    public List<Sprite> defensiveRingsImages;
    public List<Sprite> offensiveRingsImages;

    public List<Sprite> baseWeaponImages;
    public List<Sprite> baseWeaponProjectiles;


    public EquipmentSO robePrefab;
    public EquipmentSO bootsPrefab;
    public EquipmentSO glovesPrefab;
    public EquipmentSO defenseRingPrefab;
    public WeaponSO offenseRingPrefab;
    public WeaponSO weaponPrefab;

    public ItemPickUp itemPickUp;

    public void Awake()
    {
        // List Startup
        categories = new List<ItemCategory>();
        equipmentPrefixes = new List<EPrefix>();
        equipmentSuffixes = new List<ESuffix>();
        weaponPrefixes = new List<WPrefix>();
        weaponSuffixes = new List<WSuffix>();

        /* Categories
        0 - Armor (Robe)
        1 - Boots
        2 - Gloves
        3 - D Ring
        4 - O Ring
        5 - Weapon
        */

        categories.Add(new ItemCategory(("Armor"), true));
        categories[0].baseTypes = new List<BaseItemType>();
        categories[0].baseTypes.Add(new BaseEquipment("Patchy Robe", 1000f, 100f, 100f, 20f, 6f, 0, 10f));
        categories[0].baseTypes.Add(new BaseEquipment("Cotton Robe", 2000f, 200f, 200f, 30f, 7f, 0, 20f));
        categories[0].baseTypes.Add(new BaseEquipment("Cloth Robe", 3000f, 100f, 300f, 40f, 8f, 0, 30f));
        categories[0].baseTypes.Add(new BaseEquipment("Silk Robe", 4000f, 200f, 400f, 50f, 9f, 0, 40f));
        categories[0].baseTypes.Add(new BaseEquipment("Tempered Robe", 5000f, 200f, 500f, 60f, 10f, 0, 50f));

        categories[0].numberOfBases = categories[0].baseTypes.Count;

        categories.Add(new ItemCategory(("Boots"), true));
        categories[1].baseTypes = new List<BaseItemType>();
        categories[1].baseTypes.Add(new BaseEquipment("Patchy Shoes", 500f, 100f, 100f, 20f, 6f, 0, 10f));
        categories[1].baseTypes.Add(new BaseEquipment("Cotton Shoes", 1000f, 200f, 200f, 30f, 7f, 0, 20f));
        categories[1].baseTypes.Add(new BaseEquipment("Cloth Shoes", 1500f, 100f, 300f, 40f, 8f, 0, 30f));
        categories[1].baseTypes.Add(new BaseEquipment("Silk Shoes", 2000f, 200f, 400f, 50f, 9f, 0, 40f));
        categories[1].baseTypes.Add(new BaseEquipment("Tempered Shoes", 2500f, 200f, 500f, 60f, 10f, 0, 50f));
        categories[1].numberOfBases = categories[1].baseTypes.Count;

        categories.Add(new ItemCategory(("Gloves"), true));
        categories[2].baseTypes = new List<BaseItemType>();
        categories[2].baseTypes.Add(new BaseEquipment("Patchy Gloves", 500f, 100f, 100f, 20f, 6f, 0, 10f));
        categories[2].baseTypes.Add(new BaseEquipment("Cotton Gloves", 1000f, 200f, 200f, 30f, 7f, 0, 20f));
        categories[2].baseTypes.Add(new BaseEquipment("Cloth Gloves", 1500f, 100f, 300f, 40f, 8f, 0, 30f));
        categories[2].baseTypes.Add(new BaseEquipment("Silk Gloves", 2000f, 200f, 400f, 50f, 9f, 0, 40f));
        categories[2].baseTypes.Add(new BaseEquipment("Tempered Gloves", 2500f, 200f, 500f, 60f, 10f, 0, 50f));
        categories[2].numberOfBases = categories[2].baseTypes.Count;

        categories.Add(new ItemCategory(("Defensive Ring"), true));
        categories[3].baseTypes = new List<BaseItemType>();
        categories[3].baseTypes.Add(new BaseEquipment("Emerald Copper Ring", 500f, 100f, 100f, 20f, 6f, 0, 10f));
        categories[3].baseTypes.Add(new BaseEquipment("Emerald Iron Ring", 1000f, 200f, 200f, 30f, 7f, 0, 20f));
        categories[3].baseTypes.Add(new BaseEquipment("Emerald Quartz Ring", 2000f, 200f, 400f, 50f, 9f, 0, 30f));
        categories[3].baseTypes.Add(new BaseEquipment("Emerald Palladium Ring", 2000f, 200f, 400f, 50f, 9f, 0, 40f));
        categories[3].baseTypes.Add(new BaseEquipment("Emerald Cobalt Ring", 2500f, 200f, 500f, 60f, 10f, 0, 50f));
        categories[3].numberOfBases = categories[3].baseTypes.Count;

        categories.Add(new ItemCategory(("Offensive Ring"), false));
        categories[4].baseTypes = new List<BaseItemType>();
        categories[4].baseTypes.Add(new BaseWeapon("Ruby Copper Ring", 200f, 0, 0, 0, 0, 0, 0, 10f));
        categories[4].baseTypes.Add(new BaseWeapon("Ruby Iron Ring", 300f, 0, 0, 0, 0, 0, 0, 20f));
        categories[4].baseTypes.Add(new BaseWeapon("Ruby Quartz Ring", 400f, 0, 0, 0, 0, 0, 0, 30f));
        categories[4].baseTypes.Add(new BaseWeapon("Ruby Palladium Ring", 500f, 0, 0, 0, 0, 0, 0, 40f));
        categories[4].baseTypes.Add(new BaseWeapon("Ruby Cobalt Ring", 600f, 0, 0, 0, 0, 0, 0, 50f));
        categories[4].numberOfBases = categories[4].baseTypes.Count;

        categories.Add(new ItemCategory(("Weapon"), false));
        categories[5].baseTypes = new List<BaseItemType>();
        categories[5].baseTypes.Add(new BaseWeapon("Fire Staff", 500f, 0f, 0f, .15f, .25f, 0, 0, 50f));
        categories[5].baseTypes.Add(new BaseWeapon("Lightning Staff", 300f, .15f, .5f, .15f, 0f, 1, 0, 50f));
        categories[5].baseTypes.Add(new BaseWeapon("Leaf Staff", 400f, 0f, 0f, .2f, .5f, 1, 0, 40f));
        categories[5].baseTypes.Add(new BaseWeapon("Rock Staff", 800f, -.05f, .5f, .25f, 0f, 0, 0, 41f));
        categories[5].numberOfBases = categories[5].baseTypes.Count;


        /* Prefixes
        Equipment
        Weapon
        */
        equipmentPrefixes.Add(new EPrefix("Quick", .01f, 0, 0, 0, 0, .12f, 12.5f));
        equipmentPrefixes.Add(new EPrefix("Heavy", .01f, 0, 0, 0, 0, -.12f, -7.5f));
        equipmentPrefixes.Add(new EPrefix("Cowardly", -.3f, -.3f, -.3f, -.3f, -.3f, .24f, 20f));
        equipmentPrefixes.Add(new EPrefix("Solid", .01f, 0, .15f, 0, 0, 0, 11f));
        equipmentPrefixes.Add(new EPrefix("Weak", .01f, 0, -.15f, 0, 0, 0, -9f));
        equipmentPrefixes.Add(new EPrefix("Healthy", .2f, 0, 0, 0, 0, 0, 12f));
        equipmentPrefixes.Add(new EPrefix("Diseased", -.2f, 0, 0, 0, 0, 0, -8f));
        equipmentPrefixes.Add(new EPrefix("Arcane", .01f, .2f, 0, 0, 0, 0, 8));
        equipmentPrefixes.Add(new EPrefix("Inept", .01f, -.2f, 0, 0, 0, 0, -12f));
        equipmentPrefixes.Add(new EPrefix("Defensive", .01f, 0, .2f, 0, 0, 0, 12f));
        equipmentPrefixes.Add(new EPrefix("Fragile", .01f, 0, -.2f, 0, 0, 0, -8f));
        equipmentPrefixes.Add(new EPrefix("Regenerative", .01f, 0, 0, .2f, .2f, 0, 9f));
        equipmentPrefixes.Add(new EPrefix("Sickly", .01f, 0, 0, -.2f, -.2f, 0, -11f));


        weaponPrefixes.Add(new WPrefix("Slick", -.4f, .20f, 0, 0, 0, 0, 0, 20f));
        weaponPrefixes.Add(new WPrefix("Heavy", .4f, -.20f, 0, 0, 0, 0, 0, 5f));
        weaponPrefixes.Add(new WPrefix("Powerful", .20f, 0, 0, 0, 0, 0, 0, 13f));
        weaponPrefixes.Add(new WPrefix("Inferior", -.20f, 0, 0, 0, 0, 0, 0, -7f));
        weaponPrefixes.Add(new WPrefix("Destructive", .01f, 0, 0, .2f, 0, 0, 0, 15f));
        weaponPrefixes.Add(new WPrefix("Wavering", .01f, 0, 0, -.2f, 0, 0, 0, -5f));
        weaponPrefixes.Add(new WPrefix("Hailstorm", -.4f, .4f, .4f, 0, 0, 0, 0, 25f));
        weaponPrefixes.Add(new WPrefix("Forceful", .01f, 0, 0, 0, .5f, 0, 0, 6f));
        weaponPrefixes.Add(new WPrefix("Exhausted", .01f, 0, 0, 0, -.5f, 0, 0, -14f));
        weaponPrefixes.Add(new WPrefix("Sharp", .01f, 0, 0, 0, 0, 1, 0, 10f));
        weaponPrefixes.Add(new WPrefix("Dull", .01f, 0, 0, 0, 0, -1, 0, -10f));
        weaponPrefixes.Add(new WPrefix("Splitting", -.5f, 0, 0, 0, 0, 0, 2, 25f));

        /* Suffixes
        Equipment
        Weapon
        */
        equipmentSuffixes.Add(new ESuffix("of Health", .25f, 0, 0, 0, 0, 0, 25f));
        equipmentSuffixes.Add(new ESuffix("of Greater Health", .35f, 0, 0, 0, 0, 0, 40f));
        equipmentSuffixes.Add(new ESuffix("of The Giant", .50f, 0, 0, 0, 0, 0, 55f));
        equipmentSuffixes.Add(new ESuffix("of Mana", .01f, .25f, 0, 0, 0, 0, 20f));
        equipmentSuffixes.Add(new ESuffix("of Greater Mana", .01f, .35f, 0, 0, 0, 0, 35f));
        equipmentSuffixes.Add(new ESuffix("of The Wizard", .01f, .50f, 0, 0, 0, 0, 50f));
        equipmentSuffixes.Add(new ESuffix("of Guard", .01f, 0, .25f, 0, 0, 0, 20f));
        equipmentSuffixes.Add(new ESuffix("of Greater Guard", .01f, 0, .35f, 0, 0, 0, 35f));
        equipmentSuffixes.Add(new ESuffix("of The Golem", .01f, 0, .50f, 0, 0, 0, 50f));
        equipmentSuffixes.Add(new ESuffix("of Regeneration", .01f, 0, 0, .25f, .25f, 0, 19f));
        equipmentSuffixes.Add(new ESuffix("of Greater Regeneration", .01f, 0, 0, .35f, .35f, 0, 34f));
        equipmentSuffixes.Add(new ESuffix("of The Zombie", .01f, 0, 0, .5f, .5f, 0, 39f));
        equipmentSuffixes.Add(new ESuffix("of The Coward", .01f, 0, 0, 0, 0, .4f, 51f));

        weaponSuffixes.Add(new WSuffix("of Damage", .25f, 0, 0, 0, 0, 0, 0, 35f));
        weaponSuffixes.Add(new WSuffix("of The Executioner", .4f, 0, 0, 0, 0, 0, 0, 50f));
        weaponSuffixes.Add(new WSuffix("of Punch", .01f, 0, 0, 0, .5f, 0, 0, 32f));
        weaponSuffixes.Add(new WSuffix("of The Hammer", .01f, 0, 0, 0, .8f, 0, 0, 47f));
        weaponSuffixes.Add(new WSuffix("of Pierce", .01f, 0, 0, 0, 0, 1, 0, 35f));
        weaponSuffixes.Add(new WSuffix("of The Cutting", .01f, 0, 0, 0, 0, 2, 0, 50f));
        weaponSuffixes.Add(new WSuffix("of Haste", -.25f, .25f, .10f, 0, 0, 0, 0, 35f));
        weaponSuffixes.Add(new WSuffix("of The Caster", .01f, .35f, .15f, 0, 0, 0, 0, 50f));
        weaponSuffixes.Add(new WSuffix("of Launching", .01f, 0, .30f, 0, 0, 0, 0, 32f));
        weaponSuffixes.Add(new WSuffix("of The Trebuchet", .01f, 0, .60f, 0, 0, 0, 0, 47f));
        weaponSuffixes.Add(new WSuffix("of Resolution", .01f, 0, 0, .25f, 0, 0, 0, 39f));
        weaponSuffixes.Add(new WSuffix("of Absolution", .01f, 0, 0, .40f, 0, 0, 0, 54f));
        weaponSuffixes.Add(new WSuffix("of The Hailstorm", -.5f, .5f, .5f, 0, 0, 0, 0, 70f));
        weaponSuffixes.Add(new WSuffix("of Volley", -.4f, 0, 0, .0f, 0, 0, 2, 60f));
        weaponSuffixes.Add(new WSuffix("of The Hydra", -.6f, 0, 0, 0, 0, 0, 4, 90f));

    }
}
