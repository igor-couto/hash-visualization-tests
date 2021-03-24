using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bogus;

namespace random_test
{
    class Program
    {
        static void Main(string[] args)
        {
            var amount = 1000;
            if(args.Length > 0)
                amount = int.Parse(args[0]);

            var hashTable = new HashSet<int>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var collisions = 0;

            for (var i = 0; i < amount; i++) 
            {
                var generatedSeed = GenerateAvatar();

                if (hashTable.Contains(generatedSeed)) 
                    collisions++;
                else
                    hashTable.Add(generatedSeed);
            }

            stopwatch.Stop();

            Console.WriteLine($"Execution Time: {stopwatch.Elapsed}\nGenerated Avatars: {amount}\nCollisions: {collisions}\nRatio: {(float) collisions/amount * 100f}%");
        }

        static int GenerateAvatar()
        {
            var faker = new Faker("en");

            var input = faker.Person.FullName;
            
            var seed = SDBMHash(input);
            var random = new Random(seed);

            var skinTone = random.Next(0, 6);
            var hair = random.Next(0, 6);
            var hairColor = random.Next(0, 6);
            var clothing = random.Next(0, 6);

            Console.WriteLine($"Name: {input}\nSeed: {seed}\nCharacteristics: \n\tSkin: {skinTone}\n\tHair Type: {hair}\n\tHair Color: {hairColor}\n\tClothing: {clothing}\n");
            return seed;
        }

        static int GetSeedBySum(string input)
        {
            var seed = 0;
            for(var c = 0; c < input.Length; c++)
                seed += input[c];

            return seed;
        }

        static int GetSeedByXor(string input)
        {
            var seed = 0;
            for(var c = 0; c < input.Length; c++)
                seed ^= input[c];

            return seed;
        }

        static int SDBMHash(string input)
        {
            int hash = 0;

            for (int i = 0; i < input.Length; i++)
                hash = (hash << 6) + (hash << 16) - hash + ((byte)input[i]); // equivalent to: hash = 65599 * hash + (byte) input[i];

            return hash;
        }

        static int SimpleHash(string input)
        {
            int hash = 0;

            for (int i = 0; i < input.Length; i++)
                hash = (hash << 5) - hash + ((byte)input[i]); // equivalent to: hash = 31 * hash + (byte) input[i];

            return hash;
        }

        static int DiceBearHash(string seed)
        {
            var hash = 0;

            for (var i = 0; i < seed.Length; i++)
            {
                hash = ((hash << 5) - hash + seed[i]) | 0;
                hash = Xorshift(hash);
            }

            return hash;
        }

        static int Xorshift(int value)
        {
            value ^= value << 13;
            value ^= value >> 17;
            value ^= value << 5;

            return value;
        }
    }
}
