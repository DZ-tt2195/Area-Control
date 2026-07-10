using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;
using System;

public class CardType
{
    public CardData dataFile { get; private set; }

    public CardType(CardData dataFile)
    {
        this.dataFile = dataFile;
    }

#region Twist
    public virtual void TwistEffect(Player player, int logged)
    {
    }

#endregion

#region  Customer
    public virtual bool CanSell(Player player, Dictionary<TokenType, int[]> soldTokens)
    {
        return false;
    }
#endregion

#region  Checks
    protected bool SumOfLevels(Dictionary<TokenType, int[]> soldTokens, FindNumber toFind, int compare)
    {
        int totalLevels = 0;
        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
        {
            for (int i = 0; i<soldTokens[token].Length; i++)
                totalLevels += soldTokens[token][i] * i;
        }
        return MyExtensions.Comparison(toFind, totalLevels, compare); 
    }
    protected bool WithLevel(Dictionary<TokenType, int[]> soldTokens, FindNumber toFind, int specificLevel, int compare)
    {
        int withLevel = 0;
        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
            withLevel += soldTokens[token][specificLevel];
        return MyExtensions.Comparison(toFind, withLevel, compare); 
    }
    protected bool TypesInOrder(Dictionary<TokenType, int[]> soldTokens, TokenType smallType, TokenType middleType, TokenType bigType)
    {
        int minFirst = int.MaxValue;
        int maxLast = int.MinValue;
        HashSet<int> middleLevels = new();

        for (int i = 0; i<soldTokens[smallType].Length; i++)
            if (soldTokens[smallType][i] >= 1) minFirst = Mathf.Min(minFirst, i);
        for (int i = 0; i<soldTokens[bigType].Length; i++)
            if (soldTokens[bigType][i] >= 1) maxLast = Mathf.Max(maxLast, i);
        for (int i = 0; i<soldTokens[middleType].Length; i++)
            if (soldTokens[middleType][i] >= 1) middleLevels.Add(i);

        if (minFirst >= maxLast) return false;
        for (int level = minFirst + 1; level < maxLast; level++)
        {
            if (middleLevels.Contains(level))
                return true;
        }
        return false;    
    }
    protected bool SequentialLevels(Dictionary<TokenType, int[]> soldTokens, TokenType toFind, int number)
    {
        HashSet<int> uniqueLevels = new();
        for (int i = 0; i<soldTokens[toFind].Length; i++)
        {
            if (soldTokens[toFind][i] >= 1)
                uniqueLevels.Add(i);
        }
        if (uniqueLevels.Count < number) 
            return false;

        foreach (int currentLevel in uniqueLevels)
        {
            int consecutive = 1;
            while (uniqueLevels.Contains(consecutive+currentLevel))
            {
                consecutive++;
                if (consecutive == number) 
                    return true;
            }
        }
        return false;        
    }
    protected bool TypesOrNot(Dictionary<TokenType, int[]> soldTokens, int minimum, HashSet<TokenType> requiredTypes, HashSet<TokenType> bannedTypes)
    {
        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
        {
            int sum = MyExtensions.SumOfArray(soldTokens[token]);
            if (requiredTypes.Contains(token) && sum < minimum)
                return false;
            else if (bannedTypes.Contains(token) && sum != 0)
                return false;
        }
        return true;
    }
    protected bool HigherTypeVs(Dictionary<TokenType, int[]> soldTokens, int minimumTokens, TokenType highest)
    {
        int highestTargetToken = -1;
        int othersTotalLevel = 0;
        int totalTokens = 0;

        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
        {
            int[] array = soldTokens[token];
            int sum = MyExtensions.SumOfArray(soldTokens[token]);
            totalTokens += sum;

            if (token == highest)
            {
                if (sum != 1) return false;
                for (int i = array.Length-1; i>= 0; i--)
                {
                    if (array[i] >= 1)
                    {
                        highestTargetToken = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i<array.Length; i++)
                    othersTotalLevel += array[i] * i;
            }
        } 
        return totalTokens >= minimumTokens && highestTargetToken > othersTotalLevel;
    }
    protected bool NumTokensSold(Dictionary<TokenType, int[]> soldTokens, FindNumber toFind, int compare)
    {
        int totalTokens = 0;
        foreach (TokenType token in Enum.GetValues(typeof(TokenType)))
            totalTokens += MyExtensions.SumOfArray(soldTokens[token]);
        return MyExtensions.Comparison(toFind, totalTokens, compare); 
    }
#endregion

}
