
using System.Collections.Generic;

public class MarketConfigurationStaticModel : ConfigurationStaticModel
    {
       
       // private PremiumResearchRuntimeModel _researchModel;
      
        public static float maxTimeInWaitingZone = 30f;
       
        public static int maxEmployees = 12;

        public static int maxCashiers = 5;
   
        public static double initialCash = 1000.0;
      
        private float _baseClientSpeed = 2.5f;
      
        public static float searchEvery = 0.2f;
        
        public static int videoPowerUp = 2;

        public static int videoPowerUpDuration = 3600;
      
        public static int maxVideoPowerUpDuration = 36000;
   
        public static int multiplyCost = 30;
     
        public static int videoWaitTime;
     
        public static int videoBaseAmount;
   
        public static int videoStepAmount;

        public static int videoMaxSteps;
   
        public static int initialMoney;
     
        public bool isCodiZoneEnabled;
    // public float clientSpeed => _baseClientSpeed * _researchModel.peopleSpeedMultiplier;
      
}






