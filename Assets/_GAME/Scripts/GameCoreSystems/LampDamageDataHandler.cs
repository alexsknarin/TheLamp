using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;
using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems
{
    public class LampDamageDataHandler
    {
        public int MaxHealth { get; set; }
    
        private float NormalizeDamageWeight(int damageNumber, int maxHealth)
        {
            return (float)damageNumber / MaxHealth;
        }
    
        public GlassDamageData UpdateGlassDamageDataDamage(GlassDamageData glassDamageData, Vector3 impactPoint)
        {
            // Calculate the visibility of the Cracks texture based on the impact direction
            if (impactPoint.x >= 0f && impactPoint.y >= -0.5f)
            {
                glassDamageData.CracksNumberRight++;
            }
            else if (impactPoint.x < 0f && impactPoint.y >= -0.5f)
            {
                glassDamageData.CracksNumberLeft++;
            }
            else if (impactPoint.y < -0.5f)
            {
                glassDamageData.CracksNumberBottom++;
            }
            glassDamageData.CracksAmountRight = NormalizeDamageWeight(glassDamageData.CracksNumberRight, MaxHealth);
            glassDamageData.CracksAmountLeft = NormalizeDamageWeight(glassDamageData.CracksNumberLeft, MaxHealth);
            glassDamageData.CracksAmountBottom = NormalizeDamageWeight(glassDamageData.CracksNumberBottom, MaxHealth);
        
            // Update the impact points
            // There are only three impact points, so we need to cycle through them
            // every time we reach the third point, we go back to the first one
        
            glassDamageData.ImpactLastPointNumber++;
            if (glassDamageData.ImpactLastPointNumber > 6)
            {
                // We reuse 3 points for the impact points - after 6 hits all points are updated, and then we can start over
                // 1 2 3 iterations will contain zeroes that's why loop tarts on 4 
                glassDamageData.ImpactLastPointNumber = 4;
            }
        
            // Calculate current point position (global angle) and rotation (local angle) 
            float impactRandomLocalAngle = Random.Range(0, 6.2832f);
            float impactGlobalAngle = Mathf.Acos(impactPoint.x);
            if (impactPoint.y < 0)
            {
                impactGlobalAngle = Mathf.PI * 2 - impactGlobalAngle;
            }
        
            switch (glassDamageData.ImpactLastPointNumber)
            {
                case 1:
                    glassDamageData.LampDamagePoint01.Strength = 1f;
                    glassDamageData.LampDamagePoint01.LocalAngle = impactRandomLocalAngle;
                    glassDamageData.LampDamagePoint01.GlobalAngle = impactGlobalAngle;
                    break;
                case 2:
                    glassDamageData.LampDamagePoint01.Strength = 0.66f;
                    glassDamageData.LampDamagePoint02.Strength = 1f;
                    glassDamageData.LampDamagePoint02.LocalAngle = impactRandomLocalAngle;
                    glassDamageData.LampDamagePoint02.GlobalAngle = impactGlobalAngle;
                    break;
                case 3:
                    glassDamageData.LampDamagePoint01.Strength = 0.33f;
                    glassDamageData.LampDamagePoint02.Strength = 0.66f;
                    glassDamageData.LampDamagePoint03.Strength = 1f;
                    glassDamageData.LampDamagePoint03.LocalAngle = impactRandomLocalAngle;
                    glassDamageData.LampDamagePoint03.GlobalAngle = impactGlobalAngle;
                    break;
                case 4:
                    glassDamageData.LampDamagePoint01.Strength = 1f;
                    glassDamageData.LampDamagePoint02.Strength = 0.33f;
                    glassDamageData.LampDamagePoint03.Strength = 0.66f;
                    glassDamageData.LampDamagePoint01.LocalAngle = impactRandomLocalAngle;
                    glassDamageData.LampDamagePoint01.GlobalAngle = impactGlobalAngle;
                    break;
                case 5:
                    glassDamageData.LampDamagePoint01.Strength = 0.66f;
                    glassDamageData.LampDamagePoint02.Strength = 1f;
                    glassDamageData.LampDamagePoint03.Strength = 0.33f;
                    glassDamageData.LampDamagePoint02.LocalAngle = impactRandomLocalAngle;
                    glassDamageData.LampDamagePoint02.GlobalAngle = impactGlobalAngle;
                    break;
                case 6:
                    glassDamageData.LampDamagePoint01.Strength = 0.33f;
                    glassDamageData.LampDamagePoint02.Strength = 0.66f;
                    glassDamageData.LampDamagePoint03.Strength = 1f;
                    glassDamageData.LampDamagePoint03.LocalAngle = impactRandomLocalAngle;
                    glassDamageData.LampDamagePoint03.GlobalAngle = impactGlobalAngle;
                    break;
            }
            return glassDamageData;
        }

        public GlassDamageData UpdateGlassDamageDataHeal()
        {
            return new GlassDamageData();
        }
    
        public GlassDamageData UpdateGlassDamageDataHealthUpgrade()
        {
            return new GlassDamageData();
        }
    }
}
