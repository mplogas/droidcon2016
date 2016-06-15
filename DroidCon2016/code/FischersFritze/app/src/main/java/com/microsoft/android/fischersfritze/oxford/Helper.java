package com.microsoft.android.fischersfritze.oxford;

import java.util.Random;

/**
 * Created by mplogas on 13.06.16.
 */
public class Helper {
    //http://www.uebersetzung.at/twister/de.htm
    private static final String[] german = {
        "Fischers Fritze fischt frische Fische; Frische Fische fischt Fischers Fritze.",
        "Denen Dänen, denen Dänen Dänen dehnen, dehnen deren Dänen",
        "Am Zehnten Zehnten um zehn Uhr zehn zogen zehn zahme Ziegen zehn Zentner Zucker zum Zoo.",
        "Bierbrauer Bauer braut braunes Bier, braunes Bier braut Bierbrauer Bauer.",
        "Haifischschwanzflossenfleischsuppe",
        "Kalle Kahlekatzenglatzenkratzer kratzt kahle Katzenglatzen.",
        "Schnecken erschrecken, wenn Schnecken an Schnecken schlecken, weil zum Schrecken vieler Schnecken, Schnecken nicht schmecken.",
        "Brautkleid bleibt Brautkleid und Blaukraut bleibt Blaukraut.",
        "Die Katze fraß den Saumagen, nun kann sie nicht mehr Mau sagen!"
    };

    //http://www.uebersetzung.at/twister/en.htm
    private static final String[] english = {
            "I saw Susie sitting in a shoe shine shop. Where she sits she shines, and where she shines she sits.",
            "How many boards could the Mongols hoard if the Mongol hordes got bored?",
            "How can a clam cram in a clean cream can?",
            "The thirty-three thieves thought that they thrilled the throne throughout Thursday.",
            "Can you can a can as a canner can can a can?",
            "Six sick hicks nick six slick bricks with picks and sticks.",
            "If Stu chews shoes, should Stu choose the shoes he chews?",
            "How much wood could Chuck Woods' woodchuck chuck, if Chuck Woods' woodchuck could and would chuck wood?",
            "As one black bug, bled blue, black blood. The other black bug bled blue."
    };

    //http://www.uebersetzung.at/twister/fr.htm
    private static final String[] french = {
            "Je suis ce que je suis et si je suis ce que je suis, qu'est-ce que je suis?",
            "Un dragon gradé dégrade un gradé dragon.",
            "Pauvre petit pêcheur, prend patience pour pouvoir prendre plusieurs petits poissons.",
            "Ces cerises sont si sûres qu'on ne sait pas si c'en sont."
    };


    public static int levenshteinDistance(CharSequence lhs, CharSequence rhs) {
        if (lhs == null && rhs == null) {
            return 0;
        }
        if (lhs == null) {
            return rhs.length();
        }
        if (rhs == null) {
            return lhs.length();
        }

        int len0 = lhs.length() + 1;
        int len1 = rhs.length() + 1;

        // the array of distances
        int[] cost = new int[len0];
        int[] newcost = new int[len0];

        // initial cost of skipping prefix in String s0
        for (int i = 0; i < len0; i++) cost[i] = i;

        // dynamically computing the array of distances

        // transformation cost for each letter in s1
        for (int j = 1; j < len1; j++) {
            // initial cost of skipping prefix in String s1
            newcost[0] = j;

            // transformation cost for each letter in s0
            for(int i = 1; i < len0; i++) {
                // matching current letters in both strings
                int match = (lhs.charAt(i - 1) == rhs.charAt(j - 1)) ? 0 : 1;

                // computing cost for each transformation
                int cost_replace = cost[i - 1] + match;
                int cost_insert  = cost[i] + 1;
                int cost_delete  = newcost[i - 1] + 1;

                // keep minimum cost
                newcost[i] = Math.min(Math.min(cost_insert, cost_delete), cost_replace);
            }

            // swap cost/newcost arrays
            int[] swap = cost; cost = newcost; newcost = swap;
        }

        // the distance is the cost for transforming all letters in both strings
        return cost[len0 - 1];
    }

    public static String getRandomTongueTwister(String language) {
        Random r = new Random();
        String[] array;

        switch (language) {
            case "de-de":
                array = german;
                break;
            case "fr-fr":
                array = french;
                break;
            case "en-us":
            default:
                array = english;
                break;
        }

        return array[r.nextInt(array.length - 1)];
    }
}
