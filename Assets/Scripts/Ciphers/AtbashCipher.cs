﻿using CipherMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtbashCipher
{

	public PageInfo[] encrypt(string word, string id, string log)
	{
		Debug.LogFormat("{0} Begin Atbash Cipher", log);
		string encrypt = "";
		foreach (char c in word)
			encrypt = encrypt + "" + (char)(155 - c);
		Debug.LogFormat("{0} [Atbash Cipher] {1} -> {2}", log, word, encrypt);
		ScreenInfo[] screens = new ScreenInfo[9];
		for (int i = 0; i < 8; i++)
			screens[i] = new ScreenInfo();
		screens[8] = new ScreenInfo(id, 35);
		PageInfo[] page = { new PageInfo(new ScreenInfo[] { new ScreenInfo(encrypt, 35) }), new PageInfo(screens) };
		return page;
	}
}