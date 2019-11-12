using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomNameGenerator : MonoBehaviour
{
    public Text Text;
    Island Island { get { return GetComponent<Island>(); } }
    int[] samohlasky = { 'a', 'e', 'i', 'o', 'u', 'y' };
    int[] spoluhlasky = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
    int max;
    int[] slovo;
    int[] altslovo;
    int upper = 122;
    int lower = 97;
    int num = 0;
    int pass = 0;
    int spoluhlaska;
    int z = 2;
    int useslovo2 = 0;
    int[] slovo2;
    int crashcounter;

    void Start()
    {
        System.Random prng = new System.Random(Island.seed);
        max = Random.Range(0, 8) + 3;
        slovo = new int[max];
        altslovo = new int[max];
        slovo2 = new int[max + 1];
        string name = main(prng);
        if (name == "") name = main(new System.Random(Island.seed + 1));
        //Debug.Log(name);
        Text.text = name;
    }

    string main(System.Random prng)
    {
        num = (prng.Next(0, upper - lower + 1)) + lower;

        spoluhlaska = 1;
        for (int j = 0; j < 6; j++)
        {
            if (num == samohlasky[j])
            {
                spoluhlaska = 0;
            }
        }

        slovo[0] = num;
        altslovo[0] = spoluhlaska;

        if (slovo[0] == 'q')
        {
            slovo[1] = 'u';
            altslovo[1] = 0;
        }
        else
        {
            pass = 0;
            crashcounter = 0;
            while (pass == 0)
            {
                crashcounter++;
                num = (prng.Next(0, upper - lower + 1)) + lower;
                pass = 1;
                spoluhlaska = 1;
                for (int j = 0; j < 6; j++)
                {
                    if (num == samohlasky[j])
                    {
                        spoluhlaska = 0;
                    }
                }
                if (num == slovo[0] && slovo[0] == 'x' || slovo[0] == 'w')
                    pass = 0;

                if (crashcounter > 100)
                {
                    return main(prng);
                }
            }
            slovo[1] = num;
            altslovo[1] = spoluhlaska;
            if (slovo[1] == 'q')
            {
                z++;
                slovo[2] = 'u';
            }
        }


        for (int i = z; i < max; i++)
        {
            pass = 0;
            crashcounter = 0;
            while (pass == 0)
            {
                crashcounter++;
                num = (Random.Range(0,upper - lower + 1)) + lower;
                pass = 1;
                spoluhlaska = 1;
                for (int j = 0; j < 6; j++)
                {
                    if (num == samohlasky[j])
                    {
                        spoluhlaska = 0;
                    }
                }
                if (spoluhlaska == 0)
                {
                    if (altslovo[i - 1] == 0 && altslovo[i - 2] == 0)
                        pass = 0;
                }
                if (spoluhlaska == 1)
                {
                    if (altslovo[i - 1] == 1 && altslovo[i - 2] == 1)
                        pass = 0;
                }
                if (num == slovo[i - 1] && slovo[i - 1] == 'x' || slovo[i - 1] == 'w')
                    pass = 0;
                if (crashcounter > 100)
                {
                    return main(prng);
                }

            }
            slovo[i] = num;
            altslovo[i] = spoluhlaska;
            if (slovo[i] == 'q')
            {
                if (i == max - 1)
                {
                    for (int k = 0; k < max; k++)
                    {
                        slovo2[k] = slovo[k];
                    }
                    slovo2[max] = 'u';
                    useslovo2 = 1;
                }
                else
                {
                    i++;
                    slovo[i] = 'u';
                }

            }
        }
        if (useslovo2 == 1)
        {
            max++;
            string returnvalue = "";
            for (int i = 0; i < max; i++)
            {
                if (i == 0) returnvalue += ((char)slovo2[i]).ToString().ToUpper();
                else returnvalue += (char)slovo2[i];
            }
            return returnvalue;
        }
        else
        {
            string returnvalue = "";
            for (int i = 0; i < max; i++)
            {
                if (i == 0) returnvalue += ((char)slovo[i]).ToString().ToUpper();
                else returnvalue += (char)slovo[i];
            }
            return returnvalue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

