using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckoutSystem : MonoBehaviour
{
    public List<CheckoutModel> checkOutList = new List<CheckoutModel>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCheckoutIndex()
    {
        int peopleInLine = checkOutList[0]._queue.Count;

        int _index = 0;

        if (IsCheckoutQueueFull())
        {
            Debug.Log("FULL CHECKOUT");
            return -1;
        }
           

        if (checkOutList.Count < 1)
            return 0;

        for(int i = 1; i < checkOutList.Count; i ++)
        {
            if (checkOutList[i].level == 0)
                break;
            if(peopleInLine > checkOutList[i]._queue.Count)
            {
                peopleInLine = checkOutList[i]._queue.Count;
                _index = i;
            }
        }

        return _index;
    }

    public bool IsCheckoutQueueFull()
    {
        bool isFull = true;

        for (int i = 0; i < checkOutList.Count; i++)
        {
            if (checkOutList[i]._queue.Count < checkOutList[i].MaxQueue)
                isFull = false;
        }

        return isFull;
    }

    public int GetCurrentMaxCashiers()
    {
        int num = 0;
       // Debug.Log("BREAK AT " + GamePlaySytem.instance._generatorStaticModels["CHECKOUT2"].id);
        for (int i = 1; i <= MarketConfigurationStaticModel.maxCashiers; i++)
        {
           
            GeneratorStaticModel _generator = GamePlaySytem.instance._generatorStaticModels["CHECKOUT" + i.ToString()];
            if (_generator.id == null)
            {
               // Debug.Log("BREAK AT " + i);
                break;
            }    
               
            string requirement = _generator.requirement;
            //Debug.Log("NUM " + requirement);
            if (requirement == "none")
            {
                num ++;
                continue;
            }
           
            if (GamePlaySytem.instance.marketSystem._view.generatorControllers.ContainsKey(requirement))
            {
                
                GeneratorModel _model = GamePlaySytem.instance.marketSystem._view.generatorControllers[requirement];


                if (_model.level > 0)
                    num++;
               // Debug.Log("NUM " + i + " " + requirement + " " + " " + num);
            }

            
        }
       
        return num;

    }

    public int GetCurrentCountCashiers()
    {
        int _count = 0;
        for(int i = 0; i < GamePlaySytem.instance.marketSystem._view.checkOutSystem.checkOutList.Count; i++)
        {
            if (GamePlaySytem.instance.marketSystem._view.checkOutSystem.checkOutList[i].level > 0)
                _count++;
        }
        return _count;
    }


    public GeneratorStaticModel GetFirstRequerimentForNextCheckout()
    {
        for (int i = 1; i <= MarketConfigurationStaticModel.maxCashiers; i++)
        {
           
            GeneratorStaticModel generatorStaticModel = GamePlaySytem.instance._generatorStaticModels["CHECKOUT" + i];

            if (generatorStaticModel.id != null && generatorStaticModel.requirement != "none")
            {
                if (GamePlaySytem.instance.marketSystem._view.generatorControllers.ContainsKey(generatorStaticModel.requirement))
                {

                    GeneratorModel generatorModel = GamePlaySytem.instance.marketSystem._view.generatorControllers[generatorStaticModel.requirement];
                    if (generatorModel == null || generatorModel.level == 0)
                    {
                        return generatorStaticModel;
                    }
                }
                //else

                  //return generatorStaticModel;

            }
           
        }

        return null;
    }
}
