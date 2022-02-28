﻿using CipherMachine;
using System.Collections.Generic;
using UnityEngine;
using Words;

public class RedefenceTransposition
{
	public PageInfo[] encrypt(string word, string id, string log, KMBombInfo Bomb)
	{
		Debug.LogFormat("{0} Begin Redefence Transposition", log);
		string[] invert = new CMTools().generateBoolExp(Bomb);
		string key = new string("1234567".Substring(0, UnityEngine.Random.Range(2, word.Length - 1)).ToCharArray().Shuffle());
		Debug.LogFormat("{0} [Redefence Transposition] Key: {1}", log, key);
		Debug.LogFormat("{0} [Redefence Transposition] Invert Rule: {1} -> {2}", log, invert[0], invert[1]);
		int offset = 1, cursor = 1;
		string encrypt = "";
		string[] grid = new string[key.Length];
		for (int i = 0; i < grid.Length; i++)
			grid[i] = "";
		if (invert[1][0] == 'T')
		{
			grid[0] += "*";
			for (int i = 1; i < word.Length; i++)
			{
				grid[cursor] += "*";
				if (cursor == 0 || cursor == (grid.Length - 1))
					offset = -offset;
				cursor += offset;
			}
			cursor = 0;
			for(int i = 0; i < key.Length; i++)
			{
				int row = "1234567".IndexOf(key[i]);
				grid[row] = word.Substring(cursor, grid[row].Length);
				cursor += grid[row].Length;
			}
			for (int i = 0; i < grid.Length; i++)
				Debug.LogFormat("{0} [Redefence Transposition] {1}", log, grid[i]);
			encrypt = grid[0][0] + "";
			grid[0] = grid[0].Substring(1);
			offset = 1;
			cursor = 1;
			for (int i = 1; i < word.Length; i++)
			{
				encrypt = encrypt + "" + grid[cursor][0];
				grid[cursor] = grid[cursor].Substring(1);
				if (cursor == 0 || cursor == (grid.Length - 1))
					offset = -offset;
				cursor += offset;
			}
		}
		else
		{
			grid[0] = grid[0] + "" + word[0];
			for (int i = 1; i < word.Length; i++)
			{
				grid[cursor] = grid[cursor] + "" + word[i];
				if (cursor == 0 || cursor == (grid.Length - 1))
					offset = -offset;
				cursor += offset;
			}
			for (int i = 0; i < key.Length; i++)
			{
				Debug.LogFormat("{0} [Redefence Transposition] {1}", log, grid[i]);
				encrypt += grid["1234567".IndexOf(key[i])];
			}
		}
		Debug.LogFormat("{0} [Redefence Transposition] {1} - > {2}", log, word, encrypt);
		ScreenInfo[] screens = new ScreenInfo[9];
		screens[0] = new ScreenInfo(key, key.Length < 7 ? 35 : 32);
		screens[1] = new ScreenInfo(invert[0], 25);
		for (int i = 2; i < 8; i++)
			screens[i] = new ScreenInfo();
		screens[8] = new ScreenInfo(id, 35);
		return (new PageInfo[] { new PageInfo(new ScreenInfo[] { new ScreenInfo(encrypt, 35) }), new PageInfo(screens) });
	}
}