function getRandomAppearance(%gender)
{
	%appearance =
		getRandomSkinColor() TAB
		getRandomFaceName(%gender) TAB
		getRandomDecalName() TAB
		getRandomHairName(%gender) TAB
		getRandomGenericColor() TAB
		getRandomPantsColor() TAB
		getRandomPantsColor() TAB
		getRandomHairColor();
	return %appearance;
}

function getSkinColorList()
{
	%index = -1;
	%color[%index++] = "0.956863 0.878431 0.784314 1";
	%color[%index++] = "1 0.878431 0.611765 1";
	%color[%index++] = "1 0.603922 0.423529 1";
	%color[%index++] = "0.392157 0.196078 0 1"; //rare color
	%i = -1;
	while(%i++ <= %index)
	{
		%fields = setField(%fields, getFieldCount(%fields), %color[%i]);
	}
	return %fields;
}

function getRandomSkinColor()
{
	%index = -1;
	%color[%index++] = "0.956863 0.878431 0.784314";
	%color[%index++] = "1 0.878431 0.611765";
	%color[%index++] = "1 0.603922 0.423529";
	if (getRandom(1, 10) == 1) //rare color
		%color[%index++] = "0.392157 0.196078 0";
	%pick = getRandom(%index);
	%r = getmax(getmin(getWord(%color[%pick], 0) + 0.05 - getRandom() * 0.1, 1), 0);
	%g = getmax(getmin(getWord(%color[%pick], 1) + 0.05 - getRandom() * 0.1, 1), 0);
	%b = getmax(getmin(getWord(%color[%pick], 2) + 0.05 - getRandom() * 0.1, 1), 0);

	return %r SPC %g SPC %b SPC 1;
}

function getHairColorList()
{
	//Grays/"blues"
	%high = -1;
	%color[%high++] = "0.753 0.816 0.816 1";
	%color[%high++] = "0.439 0.502 0.565 1";
	%color[%high++] = "0.251 0.251 0.376 1";
	%color[%high++] = "0.125 0.063 0.188 1";
	//Browns (light)
	%color[%high++] = "0.816 0.816 0.69 1";
	%color[%high++] = "0.627 0.502 0.376 1";
	%color[%high++] = "0.376 0.251 0.251 1";
	%color[%high++] = "0.251 0.125 0.125 1";
	//Browns
	%color[%high++] = "0.878 0.69 0.376 1";
	%color[%high++] = "0.753 0.502 0.251 1";
	%color[%high++] = "0.627 0.314 0.125 1";
	%color[%high++] = "0.251 0 0 1";
	//Orange
	%color[%high++] = "1 0.502 0.251 1";
	%color[%high++] = "0.753 0.251 0 1";
	%color[%high++] = "0.502 0.125 0 1";
	%color[%high++] = "0.251 0.063 0 1";
	//Blonde
	%color[%high++] = "1 1 0.627 1";
	%color[%high++] = "1 0.753 0.251 1";
	%color[%high++] = "0.753 0.502 0 1";
	%color[%high++] = "0.502 0.251 0 1";
	//Ginger
	%color[%high++] = "1 0.251 0 1";
	%color[%high++] = "1 0.502 0 1";
	%color[%high++] = "1 0.878 0 1";
	%i = -1;
	while(%i++ <= %high)
	{
		%fields = setField(%fields, getFieldCount(%fields), %color[%i]);
	}
	return %fields;
}

function getDyedHairColorList()
{
	%high = -1;
	%color[%high++] = "0.753 1 0 1";
	%color[%high++] = "0.251 0.753 0 1";
	%color[%high++] = "0 0.502 0.251 1";
	%color[%high++] = "0 0.251 0.251 1";
	//Teal
	%color[%high++] = "0 1 0.502 1";
	%color[%high++] = "0 0.753 0.502 1";
	%color[%high++] = "0 0.502 0.502 1";
	%color[%high++] = "0 0.251 0.376 1";
	//Blues
	%color[%high++] = "0.502 0.941 1 1";
	%color[%high++] = "0 0.753 1 1";
	%color[%high++] = "0 0.251 0.753 1";
	%color[%high++] = "0.125 0 0.502 1";
	//Purple
	%color[%high++] = "0.878 0.627 1 1";
	%color[%high++] = "0.753 0.251 1 1";
	%color[%high++] = "0.502 0 0.753 1";
	//Pinks
	%color[%high++] = "1 0.753 0.753 1";
	%color[%high++] = "1 0.376 0.502 1";
	%color[%high++] = "0.753 0 0.376 1";
	%color[%high++] = "0.502 0 0.376 1";
	%i = -1;
	while(%i++ <= %high)
	{
		%fields = setField(%fields, getFieldCount(%fields), %color[%i]);
	}
	return %fields;
}

function getRandomHairColor()
{
    //Natural colors
    %fields = getHairColorList();
	//Dyed colors
	if (getRandom(1, 3) == 1) //sorta rarer
	{
		%fields = %fields TAB getDyedHairColorList();
	}

	return pickField(%fields);
}

function getGenericColorList()
{
	%color0 = "0.9 0 0 1";
	%color2 = "0.74902 0.180392 0.482353 1";
	%color3 = "0.388235 0 0.117647 1";
	%color4 = "0.133333 0.270588 0.270588 1";
	%color5 = "0 0.141176 0.333333 1";
	%color6 = "0.105882 0.458824 0.768627 1";
	%color7 = "1 1 1 1";
	%color8 = "0.0784314 0.0784314 0.0784314 1";
	%color9 = "0.92549 0.513726 0.678431 1";
	%color10 = "0 0.5 0.25 1";
	%color11 = "0.784314 0.921569 0.490196 1";
	%color12 = "0.541176 0.698039 0.552941 1";
	%color13 = "0.560784 0.929412 0.960784 1";
	%color14 = "0.698039 0.662745 0.905882 1";
	%color15 = "0.878431 0.560784 0.956863 1";
	%color16 = "0.888 0 0 1";
	%color17 = "1 0.5 0 1";
	%color18 = "0.99 0.96 0 1";
	%color19 = "0.2 0 0.8 1";
	%color20 = "0 0.471 0.196 1";
	%color21 = "0 0.2 0.64 1";
	%color22 = "0.596078 0.160784 0.392157 1";
	%color23 = "0.55 0.7 1 1";
	%color24 = "0.85 0.85 0.85 1";
	%color25 = "0.1 0.1 0.1 1";
	%color27 = "0.75 0.75 0.75 1";
	%color28 = "0.5 0.5 0.5 1";
	%color29 = "0.2 0.2 0.2 1";
	%color30 = "0.901961 0.341176 0.0784314 1";

	%i = -1;
	while(%i++ <= 30)
	{
		%fields = setField(%fields, getFieldCount(%fields), %color[%i]);
	}
	return %fields;
}

function getRandomGenericColor()
{
	return pickField(getGenericColorList());
}

function getPantsColorList()
{
	%color0 = "0.75 0.75 0.75 1";
	%color1 = "0.2 0.2 0.2 1";
	%color2 = "0.388 0 0.117 1";
	%color3 = "0.133 0.27 0.27 1";
	%color4 = "0 0.141 0.333 1";
	%color5 = "0.078 0.078 0.078 1";
	%i = -1;
	while(%i++ <= 5)
	{
		%fields = setField(%fields, getFieldCount(%fields), %color[%i]);
	}
	return %fields;
}

function getRandomPantsColor()
{
	return pickField(getPantsColorList());
}


//Some faces are from winterbite face pack located here: https://www.dropbox.com/s/misn71lpawwi8le/Face_WinterBite.zip
function getFaceList(%gender)
{
	%high = -1;
	%choice[%high++] = "smiley";
	if (%gender $= "male")
	{
		%choice[%high++] = "Male07Smiley";
		%choice[%high++] = "BrownSmiley";
		%choice[%high++] = "memeHappy";
		%choice[%high++] = "memeBlockMongler";
		// %choice[%high++] = "smileyBlonde";
		%choice[%high++] = "smileyCreepy";
		//Winterbite faces:
		%choice[%high++] = "smileyST";
		%choice[%high++] = "kleinerSmiley";
		%choice[%high++] = "kleinerSmiley2ST";
		%choice[%high++] = "kleinerSmiley2";
	}
	else if(%gender $= "female")
	{
		//Winterbite faces:
		%choice[%high++] = "smileyfST";
		%choice[%high++] = "smileyfCreepy";
		%choice[%high++] = "smileyf";
		%choice[%high++] = "KleinerfSmileysST";
		%choice[%high++] = "KleinerfSmiley";
	}
	%i = -1;
	while(%i++ <= %high)
	{
		%fields = setField(%fields, getFieldCount(%fields), %choice[%i]);
	}

	return %fields;
}

function getRandomFaceName(%gender)
{
	return pickField(getFaceList(%gender));
}

function getDecalList()
{
	%high = -1;

	%choice[%high++] = "";
	%choice[%high++] = "Mod-Suit";
	%choice[%high++] = "Mod-Pilot";
	%choice[%high++] = "Mod-Army";
	%choice[%high++] = "Meme-Mongler";
	%choice[%high++] = "Medieval-YARLY";
	%choice[%high++] = "Medieval-Rider";
	%choice[%high++] = "Medieval-ORLY";
	%choice[%high++] = "Medieval-Lion";
	%choice[%high++] = "Medieval-Eagle";
	%choice[%high++] = "Hoodie";
	%choice[%high++] = "Alyx";
	%i = -1;
	while(%i++ <= %high)
	{
		%fields = setField(%fields, getFieldCount(%fields), %choice[%i]);
	}

	return %fields;
}

function getRandomDecalName()
{
	return pickField(getDecalList());
}

function getRandomHairName(%gender)
{
	%fields = getHairList(%gender);

	if (getRandom(1, 2) == 1) //rare hairs
	{
		%fields = %fields TAB getHairList("rare");
	}
	return pickField(%fields);
}

function getHairList(%gender)
{
	%high = -1;
	//Unisex hairs
	%choice[%high++] = "hair_wig";
	if (%gender $= "male") //Male hairs
	{
		%choice[%high++] = "hair_cornrows";
		%choice[%high++] = "hair_familiar";
		%choice[%high++] = "hair_layered";
		%choice[%high++] = "hair_messy";
		%choice[%high++] = "hair_neat";
		%choice[%high++] = "hair_suav";
		%choice[%high++] = "hair_mullet";
		%choice[%high++] = "hair_pompadour";
		%choice[%high++] = "hair_punk";
		%choice[%high++] = "hair_shaggy";
		%choice[%high++] = "hair_fabio";
	}
	else if (%gender $= "female")//Female hairs
	{
		%choice[%high++] = "hair_broad";
		%choice[%high++] = "hair_bunn";
		%choice[%high++] = "hair_headband";
		%choice[%high++] = "hair_ponytail";
		%choice[%high++] = "hair_daphne";
		%choice[%high++] = "hair_velma";
		%choice[%high++] = "hair_mahiru";
		%choice[%high++] = "hair_maya";
	}
	else if (%gender $= "rare")//Rare unisex
	{
		%choice[%high++] = "hair_mohawk";
		%choice[%high++] = "hair_rocker";
	}
	%i = -1;
	while(%i++ <= %high)
	{
		%fields = setField(%fields, getFieldCount(%fields), %choice[%i]);
	}

	return %fields;
}


//todo: fix this to new system
function getRandomSpecialChar(%char)
{
	//%char[%chars++-1] = "Name\tChance\tGender\tSkin\tFace\tDecal\tHair\tShirt\tPants\tShoes\tHair";
	%char[%chars++-1] = "William T. Riker\t1\tM\t1 0.8 0.6 1\tMale07Smiley\tSpace-New\thair_messy\t0.7 0.1 0.1 1\t0.1 0.1 0.1 1\t0.1 0.1 0.1 1\t0.05 0.025 0 1";
	%char[%chars++-1] = "Mr. T\t1\tM\t0.4 0.2 0 1\tBrownSmiley\tMedieval-Tunic\thair_mohawk\t0 0 0 1\t0.8 0 0 1\t0.2 0.2 0.2 1\t0 0 0 1";
	%char[%chars++-1] = "Jamie Hyneman\t1\tM\t1 0.8 0.6 1\tJamie\tAAA-None\tscoutHat\t1 1 1 1\t0.2 0.2 0.2 1\t0.4 0.2 0 1\t0.1 0.1 0.1 1";
	%char[%chars++-1] = "Adam Savage\t1\tM\t1 0.8 0.6 1\tAdamSavage\tAAA-None\theadSkin\t0.2 0.2 0.2 1\t0.2 0.2 0.2 1\t0.4 0.2 0 1\t1 0.8 0.6 1";
	%char[%chars++-1] = "Jerry McLarry\t1\tM\t1 0.8 0.6\tmemePBear\tHoodie\tscoutHat\t0.7 0.2 0.5\t0 0.1 0.3\t0.4 0.2 0\t1 1 1";
	for (%i = 0; %i < %chars; %i++)
		%chance += getField(%char[%i], 1);
	%choose = getRandom() * %chance;
	for (%i = 0; %i < %chars; %i++)
	{
		%choose -= getField(%char[%i], 1);
		if (%choose <= 0)
		{
			%choose = %char[%i];
			break;
		}
	}
	%char.name = getField(%choose, 0);
	if(getField(%choose, 2) $= "M")
		%char.gender = "male";
	else
		%char.gender = "female";

	%char.appearance = getFields(%choose, 3, 10);
}

function getCharacterName(%char, %ignoreDisguised, %revealFakeName)
{
	%name = %char.name;
	%player = %char.player;
	if(isObject(%player))
	{
		if(!%ignoreDisguised && isObject(%hat = %player.tool[%player.hatSlot]) && %hat.disguise && isObject(%img = %player.getMountedImage(2)) && %img == nameToID(%hat.image))
		{
			%name = "Unknown";
			if(%hat.disguiseName !$= "")
				%name = %hat.disguiseName;
		}
		if(%player.mangled)
			%name = "Unknown";
		if(%player.fakeName !$= "")
			if(%revealFakeName)
				%name = %name SPC "(as" SPC %player.fakeName @ ")";
			else
				%name = %player.fakeName;
	}
	return %name;
}