﻿using CipherMachine;
using System.Collections.Generic;
using UnityEngine;
using Words;

public class ScytaleTransposition
{
	public PageInfo[] encrypt(string word, string id, string log, KMBombInfo Bomb)
	{
		Debug.LogFormat("{0} Begin Scytale Transposition", log);
		CMTools cm = new CMTools();
		int[] key = cm.generateValue(Bomb);
		string[] invert = cm.generateBoolExp(Bomb);
		int rows = (key[1] % (word.Length - 2)) + 2;
		Debug.LogFormat("{0} [Scytale Transposition] Number Rows: {1} -> {2} -> {3}", log, (char)key[0], key[1], rows);
		Debug.LogFormat("{0} [Scytale Transposition] Invert Rule: {1} -> {2}", log, invert[0], invert[1]);
		string encrypt = "";
		string[] grid = new string[rows];
		for (int i = 0; i < grid.Length; i++)
			grid[i] = "";
		if (invert[1][0] == 'T')
		{
			for (int i = 0; i < word.Length; i++)
				grid[i % rows] += "*";
			int cur = 0;
			for (int i = 0; i < grid.Length; i++)
			{
				for (int j = 0; j < grid[i].Length; j++)
					grid[i] = grid[i].Substring(0, j) + "" + word[cur++] + "" + grid[i].Substring(j + 1);
				Debug.LogFormat("{0} [Scytale Transposition] {1}", log, grid[i]);
			}
			for (int i = 0; i < word.Length; i++)
				encrypt = encrypt + "" + grid[i % rows][i / rows];
		}
		else
		{
			for (int i = 0; i < word.Length; i++)
				grid[i % rows] = grid[i % rows] + "" + word[i];
			for (int i = 0; i < grid.Length; i++)
			{
				Debug.LogFormat("{0} [Scytale Transposition] {1}", log, grid[i]);
				encrypt += grid[i];
			}
		}

		Debug.LogFormat("{0} [Scytale Transposition] {1} - > {2}", log, word, encrypt);
		ScreenInfo[] screens = new ScreenInfo[9];
		screens[0] = new ScreenInfo(((char)key[0]) + "", 35);
		screens[1] = new ScreenInfo(invert[0], 25);
		for (int i = 2; i < 8; i++)
			screens[i] = new ScreenInfo();
		screens[8] = new ScreenInfo(id, 35);
		return (new PageInfo[] { new PageInfo(new ScreenInfo[] { new ScreenInfo(encrypt, 35) }), new PageInfo(screens) });
	}
}