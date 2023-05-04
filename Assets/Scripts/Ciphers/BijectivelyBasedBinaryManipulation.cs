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
        if (IsInvert)
        {
            var attemptCount = 1;
            var flipBinary = CMTools.generateBoolExp(bomb); // Used for later, 
        retryGenDecrypt:
            relevantLoggings.Clear();
            var startingValue = rnd.Range(1, 5); // Pick a random initial value.
            long endingValue = startingValue;
            for (var x = 0; x < word.Length; x++)
            {
                endingValue *= 26;
                endingValue += alphabet.IndexOf(word[x]) + 1;
                relevantLoggings.Add(string.Format("* 26 + {0} ({1}) = {2}", "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(word[x]) + 1, word[x], endingValue));
            }
            var binaryWord = "";
            var binaryLength = rnd.Range(5, 8);
            var binaryTape = "";
            for (var x = 0; x < binaryLength; x++)
                binaryTape += rnd.value < 0.5f ? '0' : '1';
            for (var x = endingValue; x > 0; x >>= 1)
                binaryWord = (x % 2).ToString() + binaryWord;
            if (binaryWord.Length > 5 * word.Length)
            {
                attemptCount++;
                goto retryGenDecrypt;
            }
            relevantLoggings.Insert(0, string.Format("{0} attempt{1} taken.", attemptCount, attemptCount == 1 ? "" : "s"));
            relevantLoggings.Insert(1, string.Format("Starting value: {0}", startingValue));
            while (binaryWord.Length < 5 * word.Length)
                binaryWord = "0" + binaryWord;
            var valueBinaryTape = 0;
            for (var x = 0; x < binaryTape.Length; x++)
                valueBinaryTape = (valueBinaryTape << 1) | (binaryTape[x] == '1' ? 1 : 0);
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
            var selectedWord = new Data().PickWord(rnd.Range(3, 9));
            var alphaPosWordSel = selectedWord.Select(a => alphabet.IndexOf(a) + 1);
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
                relevantLoggings.Add(string.Format("{0} Pass: {1}", new[] { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th" }[x], valAtLeast26 ? "Invert applied" : "No invert"));
                relevantLoggings.Add(string.Format("{0} Pass: {1} = {2}, Remaining binary string after global shift: {3}", new[] { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th" }[x], valAtLeast26 ? string.Format("{0} (!{1})", nextBinary.Select(a => a == '1' ? '0' : '1').Join("") , nextBinary) : nextBinary, alphabet[valueObtained], binaryWord.Any() ? binaryWord : "<empty>"));
            }
            relevantDisplayTexts.Add(string.Format("{0}/{1}", startingValue, initIdxPos));
            relevantDisplayTexts.Add(flipBinary.Expression);
            relevantDisplayTexts.Add(valueBinaryTape.ToString());
            relevantDisplayTexts.Add(binaryTape.Length.ToString());
            relevantDisplayTexts.Add(selectedWord);
            relevantDisplayTexts.Add("");
            relevantDisplayTexts.Add(binaryInvert);

            outputtingResults.Encrypted = encryptedResult;
        }
        else
        {
            var binaryDecrypted = "";
        }
        outputtingResults.Pages = new[] { new PageInfo(relevantDisplayTexts.Select(a => new ScreenInfo(a)).ToArray(), inverted) };

        outputtingResults.LogMessages = relevantLoggings;
        return outputtingResults;
    }
}
