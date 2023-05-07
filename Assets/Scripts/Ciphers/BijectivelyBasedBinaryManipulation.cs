using System.Collections;
using System.Collections.Generic;
using System.Linq;
using rnd = UnityEngine.Random;
using CipherMachine;
using Words;

public class BijectivelyBasedBinaryManipulation : CipherBase {

    private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public override string Name { get { return IsInvert ? "Reversed Bijectively Based Binary Manipulation" : "Bijectively Based Binary Manipulation"; } }
    public override string Code { get { return "BB"; } }
    bool inverted;
    public override bool IsInvert { get { return inverted; } }
    public BijectivelyBasedBinaryManipulation(bool invert) { inverted = invert; }
    public override ResultInfo Encrypt(string word, KMBombInfo bomb)
    {
        var outputtingResults = new ResultInfo();
        var relevantDisplayTexts = new List<string>();
        var relevantLoggings = new List<string>();
        outputtingResults.Score = 15;
        // Generation for the binary tape. Used for later.
        var binaryTapeLength = rnd.Range(5, 8);
        string binaryTape;
        do
        {
            binaryTape = "";
            for (var x = 0; x < binaryTapeLength; x++)
                binaryTape += rnd.value < 0.5f ? '0' : '1';
        }
        while (binaryTape.Count(a => a == '1') < 2 || binaryTape.Count(a => a == '0') < 2); // Try to generate a state where at least two 1s and two 0s are present in the binary string.
        var valueBinaryTape = 0;
        for (var x = 0; x < binaryTape.Length; x++)
            valueBinaryTape = (valueBinaryTape << 1) | (binaryTape[x] == '1' ? 1 : 0);
        // Bool expression to reverse the 5n binary string, used for later.
        var flipBinary = CMTools.generateBoolExp(bomb);
        // Global offsets.
        var selectedWord = new Data().PickWord(rnd.Range(3, 9));
        var alphaPosWordSel = selectedWord.Select(a => alphabet.IndexOf(a) + 1);

        var attemptCount = 1;
        if (IsInvert)
        {
        retryDecrypt:
            relevantLoggings.Clear();
            var startingValue = rnd.Range(1, 5); // Pick a random initial value between 1-4.
            long endingValue = startingValue;
            for (var x = 0; x < word.Length; x++)
            {
                endingValue *= 26;
                endingValue += alphabet.IndexOf(word[x]) + 1;
                relevantLoggings.Add(string.Format("* 26 + {0} ({1}) = {2}", "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(word[x]) + 1, word[x], endingValue));
            }
            // Convert the result into binary.
            var binaryWord = "";
            for (var x = endingValue; x > 0; x >>= 1)
                binaryWord = (x % 2).ToString() + binaryWord;
            if (binaryWord.Length > 5 * word.Length) // If the length of the binary is greater than 40 digits, try the procedure again.
            {
                attemptCount++;
                goto retryDecrypt;
            }
            relevantLoggings.Insert(0, string.Format("{0} attempt{1} taken.", attemptCount, attemptCount == 1 ? "" : "s"));
            relevantLoggings.Insert(1, string.Format("Starting value: {0}", startingValue));
            while (binaryWord.Length < 5 * word.Length)
                binaryWord = "0" + binaryWord;
            relevantLoggings.Add(string.Format("{0}: {1}", flipBinary.Expression, flipBinary.Value));
            relevantLoggings.Add(string.Format("{0} = {1}", endingValue, binaryWord));
            if (flipBinary.Value)
            {
                binaryWord = binaryWord.Reverse().Join("");
                relevantLoggings.Add(string.Format("After reversing: {0}", binaryWord));
            }
            else
                relevantLoggings.Add("Binary not reversed.");
            relevantLoggings.Add(string.Format("Read Tape: {0} = {1} (Pad {2} digits)", valueBinaryTape, binaryTape, binaryTape.Length));
            relevantLoggings.Add(string.Format("Global Shifts: {0} = {1}", selectedWord, alphaPosWordSel.Join(", ")));
            var curIdxTapeRead = 0;
            var initIdxPos = rnd.Range(0, 5 * word.Length);
            var binaryInvert = "";
            var encryptedResult = "";
            binaryWord = binaryWord.Substring(initIdxPos) + binaryWord.Substring(0, initIdxPos);
            for (var x = 0; x < word.Length; x++)
            {
                var nextBinary = "";
                while (nextBinary.Length < 5)
                {
                    if (binaryTape[curIdxTapeRead] == '1')
                    {
                        nextBinary += binaryWord.First();
                        binaryWord = binaryWord.Substring(1);
                    }
                    else
                        binaryWord = binaryWord.Substring(1) + binaryWord.Substring(0, 1);
                    curIdxTapeRead = (curIdxTapeRead + 1) % binaryTape.Length;
                }
                var valueObtained = 0;
                for (var p = 0; p < 5; p++)
                    valueObtained = (valueObtained << 1) | (nextBinary[p] == '1' ? 1 : 0);
                var valAtLeast26 = valueObtained >= 26;
                var invValAtLeast26 = (valueObtained ^ 31) >= 26;
                var possibleCharInverts = new List<char>();
                if (!valAtLeast26) possibleCharInverts.Add('0');
                if (!invValAtLeast26) possibleCharInverts.Add('1');
                var pickedChar = possibleCharInverts.PickRandom();

                binaryInvert += pickedChar;
                if (pickedChar == '1')
                    valueObtained ^= 31;
                encryptedResult += alphabet[valueObtained];
                if (binaryWord.Any())
                {
                    var curGlobalOffset = alphaPosWordSel.ElementAt(x % alphaPosWordSel.Count()) % binaryWord.Length;
                    binaryWord = binaryWord.Substring(curGlobalOffset) + binaryWord.Substring(0, curGlobalOffset);
                }
                relevantLoggings.Add(string.Format("{0} Pass: {1}", new[] { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th" }[x], pickedChar == '1' ? "Invert applied" : "No invert"));
                relevantLoggings.Add(string.Format("{0} Pass: {1} = {2}, Remaining binary string after global shift: {3}", new[] { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th" }[x], pickedChar == '1' ? string.Format("{0} (!{1})", nextBinary.Select(a => a == '1' ? '0' : '1').Join("") , nextBinary) : nextBinary, alphabet[valueObtained], binaryWord.Any() ? binaryWord : "<empty>"));
            }
            relevantDisplayTexts.Add(string.Format("{0}/{1}", startingValue, initIdxPos));
            relevantDisplayTexts.Add(binaryInvert);

            outputtingResults.Encrypted = encryptedResult;
        }
        else
        {
        retryEncrypt:
            relevantLoggings.Clear();
            var binaryDecrypted = "";
            var inversionString = "";
            for (var x = 0; x < word.Length; x++)
            {
                var curBinary = "";
                var idxCurLetter = alphabet.IndexOf(word[x]);
                for (var p = 0; p < 5; p++)
                    curBinary = ((idxCurLetter >> p) % 2 == 1 ? "1" : "0") + curBinary;
                var valUnder26 = idxCurLetter < 26;
                var invValUnder26 = (idxCurLetter ^ 31) < 26;
                relevantLoggings.Add(string.Format("{0} -> {1}", word[x], curBinary));
                if (valUnder26 && invValUnder26)
                {
                    var selectedInvert = rnd.value < 0.5f;
                    inversionString += selectedInvert ? "1" : "0";
                    binaryDecrypted += curBinary.Select(a => selectedInvert ^ a == '1' ? '1' : '0').Join("");
                }
                else if (invValUnder26)
                {
                    binaryDecrypted = curBinary.Select(a => a == '0' ? '1' : '0').Join("");
                    inversionString += '1';
                }
                else
                {
                    binaryDecrypted += curBinary;
                    inversionString += '0';
                }
            }
            relevantLoggings.Add(string.Format("Inversion String: {0}" , inversionString));
            relevantLoggings.Add(string.Format("Resulting Binary String: {0}", binaryDecrypted));
            relevantLoggings.Add(string.Format("Read Tape: {0} = {1} (Pad {2} digits)", valueBinaryTape, binaryTape, binaryTape.Length));
            relevantLoggings.Add(string.Format("Global Shifts: {0} = {1}", selectedWord, alphaPosWordSel.Join(", ")));
            var reconstructedBinary = "";
            var possible1StartIdxes = Enumerable.Range(0, binaryTape.Length).Where(a => binaryTape[a] == '1');
            var curIdxInstTape = possible1StartIdxes.ElementAt((binaryDecrypted.Length - 1) % possible1StartIdxes.Count());
            var curIdxGlobalOffset = (word.Length - 1) % alphaPosWordSel.Count();
            relevantLoggings.Add(string.Format("Starting at idx {0} on the instruction tape.", curIdxInstTape));
            relevantLoggings.Add(string.Format("Starting at idx {0} on the global modifier.", curIdxGlobalOffset));

            var originalBinaryLength = binaryDecrypted.Length;
            var curPass = 0;
            var ordials = new[] { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th" };
            while (reconstructedBinary.Length < originalBinaryLength || curIdxInstTape < 0)
            {
                if (curIdxInstTape < 0)
                {
                    curIdxInstTape += binaryTape.Length;
                    relevantLoggings.Add(string.Format("{0} Loop on instruction tape: {1}", ordials[curPass], reconstructedBinary));
                    curPass++;
                }
                switch (binaryTape[curIdxInstTape])
                {
                    case '1':
                        if (reconstructedBinary.Length % 5 == 0) // Perform the global offset.
                        {
                            curIdxGlobalOffset--;
                            if (curIdxGlobalOffset < 0)
                                curIdxGlobalOffset += alphaPosWordSel.Count();
                            if (reconstructedBinary.Any())
                            {
                                var adjustedCurGlobalOffsetVal = alphaPosWordSel.ElementAt(curIdxGlobalOffset) % reconstructedBinary.Length;
                                reconstructedBinary = reconstructedBinary.Substring(reconstructedBinary.Length - adjustedCurGlobalOffsetVal) + reconstructedBinary.Substring(0, reconstructedBinary.Length - adjustedCurGlobalOffsetVal);
                            }
                        }
                        reconstructedBinary = binaryDecrypted.Last().ToString() + reconstructedBinary;
                        binaryDecrypted = binaryDecrypted.Substring(0, binaryDecrypted.Length - 1);
                        break;
                    case '0':
                        reconstructedBinary = reconstructedBinary.Substring(reconstructedBinary.Length - 1) + reconstructedBinary.Substring(0, reconstructedBinary.Length - 1);
                        break;
                }
                curIdxInstTape--;
            }
            relevantLoggings.Add(string.Format("Reconstructed Binary String: {0}", reconstructedBinary));
            // Determine if any shifts are possible to obtain a binary string.
            var allowedOffsets = new List<int>();
            for (var x = 0; x < reconstructedBinary.Length; x++)
            {
                var adjustedBinaryString = reconstructedBinary.Substring(reconstructedBinary.Length - x) + reconstructedBinary.Substring(0, reconstructedBinary.Length - x);
                if (flipBinary.Value)
                    adjustedBinaryString = adjustedBinaryString.Reverse().Join("");
                long resultingValue = 0;
                for (var p = 0; p < adjustedBinaryString.Length; p++)
                {
                    resultingValue <<= 1;
                    if (adjustedBinaryString[p] == '1')
                        resultingValue |= 1;
                }
                long currentValue = resultingValue;
                var values = new List<int>();
                while (currentValue > 0)
                {
                    var nextValue = currentValue % 26;
                    if (nextValue == 0)
                    {
                        values.Add(26);
                        currentValue -= 26;
                    }
                    else
                        values.Add((int)nextValue);
                    currentValue /= 26;
                }
                if (values.Count - 1 == word.Length && values.Last() > 0 && values.Last() < 5)
                    allowedOffsets.Add(x);
            }

            if (!allowedOffsets.Any())
            {
                attemptCount++;
                goto retryEncrypt;
            }
            // Pick a valid offset at random and perform the procedure as normal.
            var pickedOffset = allowedOffsets.PickRandom();
            relevantLoggings.Add(string.Format("Offset picked (Out of {0} possible offsets): {1}", allowedOffsets.Count, pickedOffset));
            
            var finalBinaryString = reconstructedBinary.Substring(reconstructedBinary.Length - pickedOffset) + reconstructedBinary.Substring(0, reconstructedBinary.Length - pickedOffset);
            relevantLoggings.Add(string.Format("Shifted binary: {0}", finalBinaryString));
            relevantLoggings.Add(string.Format("{0}: {1}", flipBinary.Expression, flipBinary.Value));
            if (flipBinary.Value)
            {
                finalBinaryString = finalBinaryString.Reverse().Join("");
                relevantLoggings.Add(string.Format("After reversing: {0}", finalBinaryString));
            }
            else
                relevantLoggings.Add(string.Format("Binary is not reversed."));
            long currentValueFinal = 0;
            for (var p = 0; p < finalBinaryString.Length; p++)
            {
                currentValueFinal <<= 1;
                if (finalBinaryString[p] == '1')
                    currentValueFinal |= 1;
            }
            relevantLoggings.Add(string.Format("{0} = {1}", finalBinaryString, currentValueFinal));
            var valuesFinal = new List<int>();
            while (currentValueFinal > 0)
            {
                var nextValue = currentValueFinal % 26;
                if (nextValue == 0)
                {
                    valuesFinal.Add(26);
                    currentValueFinal -= 26;
                }
                else
                    valuesFinal.Add((int)nextValue);
                currentValueFinal /= 26;
                relevantLoggings.Add(string.Format("% 26 = {0}, / 26 = {1}", nextValue, currentValueFinal));
            }

            outputtingResults.Encrypted = valuesFinal.Take(word.Length).Reverse().Select(a => alphabet[a - 1]).Join("");


            relevantLoggings.Insert(0, string.Format("{0} attempt{1} taken to encrypt.", attemptCount, attemptCount == 1 ? "" : "s"));

            relevantDisplayTexts.Add(string.Format("{0}/{1}", valuesFinal.Last(), pickedOffset));
            relevantDisplayTexts.Add(inversionString);

        }
        relevantDisplayTexts.Insert(1, flipBinary.Expression);
        relevantDisplayTexts.Insert(2, valueBinaryTape.ToString());
        relevantDisplayTexts.Insert(3, binaryTape.Length.ToString());
        relevantDisplayTexts.Insert(4, selectedWord);
        relevantDisplayTexts.Insert(5, "");
        outputtingResults.Pages = new[] { new PageInfo(relevantDisplayTexts.Select(a => new ScreenInfo(a)).ToArray(), inverted) };

        outputtingResults.LogMessages = relevantLoggings;
        return outputtingResults;
    }
}
