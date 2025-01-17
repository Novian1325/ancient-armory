using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RpgDB;

namespace AncientArmory
{
    public sealed class BattlefieldController : PoolController
    {
        bool InitialLoadComplete;
        List<GameObject> MercPool;
        float twoSecondsFromNow;
        float fiveSecondsFromNow;
        float tenSecondsFromNow;


        Character merc_1;
        Character merc_2;

        void Start()
        {
            base.Start();
            tenSecondsFromNow = Time.time + 10;
            fiveSecondsFromNow = Time.time + 5;
            twoSecondsFromNow = Time.time + 2;
        }

        void Update()
        {
            if (Battlefield.transform.childCount != 0)
            {
                if (!InitialLoadComplete && Time.time > twoSecondsFromNow)
                {
                    merc_1 = getPoolContents(Battlefield)[0].GetComponent<Character>();
                    merc_2 = getPoolContents(Battlefield)[1].GetComponent<Character>();
                    merc_1.Right_Hand = GameDatabase.Weapons.GetByName("Bow");
                    merc_2.Right_Hand = GameDatabase.Weapons.GetByName("Longsword");
                    merc_1.Armor = GameDatabase.Armor.GetByName("Ceremonial plate, troop");
                    InitialLoadComplete = true;
                    twoSecondsFromNow = Time.time + 2;
                }
                if (Time.time > twoSecondsFromNow)
                {
                    merc_1 = merc_1 ?? getPoolContents(Battlefield)[0].GetComponent<Character>();
                    merc_2 = merc_2 ?? getPoolContents(Battlefield)[1].GetComponent<Character>();
                    if(merc_1.Hit_Points() > merc_2.Damage_Taken)
                        ResolveAttack(merc_1, merc_2);
                    if(merc_2.Hit_Points() > merc_2.Damage_Taken)
                        ResolveAttack(merc_2, merc_1);
                }
            }
        }

        void OffenseMovement(GameObject characterObject)
        {
            
        }

        void ResolveAttack(Character attacker, Character defender)
        {
            int defender_health = defender.Hit_Points() - defender.Damage_Taken;
            if (defender_health > 0)
            {
                int damage = resolveSingleOrDualWeildAttack(attacker, defender.KAC());
                Debug.Log(String.Format("Attack causes {1} damage!", attacker.Armor.Name, damage.ToString()));
                defender.Damage_Taken += damage;
                defender_health = defender.Hit_Points() - defender.Damage_Taken;
                Debug.Log(String.Format("Defender has {0} health remaining!", defender_health.ToString()));
            }
        }

        // 
        //
        // Helper Functions

        int resolveSingleOrDualWeildAttack(Character attacker, int defense)
        {
            int damage = 0;
            if (attacker.Right_Hand.Name != "None")
                damage = attacker.Attack(attacker.Right_Hand, defense);
            if (attacker.Left_Hand.Name != "None" || attacker.Right_Hand.Name == "None")
                damage = attacker.Attack(attacker.Left_Hand, defense);
            return damage;
        }
    }
}