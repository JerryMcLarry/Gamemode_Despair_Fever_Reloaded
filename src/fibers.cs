datablock StaticShapeData(strandDecal)
{
	shapeFile = $Despair::Path @ "res/shapes/strand.dts";
	canClean = true;
};

function Player::spawnFiber(%this, %color)
{
	if(getWordCount(%color) < 3)
	{
		%char = %this.character;
		%app = %char.appearance;
		%hairName = getField(%app, 3);
		%shirtColor = getField(%app, 4);
		%pantsColor = getField(%app, 5);
		%shoesColor = getField(%app, 6);
		%hairColor = getField(%app, 7);
		if(isObject(%this.getMountedImage(1)) && %this.getMountedImage(1).item.hideAppearance)
		{
			%shirtColor = "";
			%pantsColor = "";
			%shoesColor = "";
		}
		if(isObject(%hat = %this.tool[%this.hatSlot]) && isObject(%this.getMountedImage(2)) && %this.getMountedImage(2) == nameToID(%hat.image))
		{
			if(%hat.hideHair || %hairName $= "")
				%hairColor = "";
		}

		//If color is null, dont tabulate it
		%colors = (%shirtColor !$= "" ? %shirtColor @ "\t" : "") @ (%pantsColor !$= "" ? %pantsColor @ "\t" : "") @
				  (%shoesColor !$= "" ? %shoesColor @ "\t" : "") @ (%hairColor !$= "" ? %hairColor @ "\t" : "");
		%color = getField(%colors, getRandom(0, getFieldCount(%colors) - 1));
	}
	else
	{
		if(getWordCount(%color) == 3)
			%color = %color SPC "1"; //alpha channel
	}

	//Seems like they're fully obscured
	if(%color $= "")
		return;

	%pos = vectorAdd(%this.getPosition(), "0 0 1");
	%end = VectorSub(%pos, vectorSpread("0 0 5", 0.1));

	%ray = containerRayCast(%pos, %end, $SprayBloodMask); //Fibers fall

	if(%ray)
	{
		%size = 0.5 + getRandom();
		%decal = spawnDecal(strandDecal, getWords(%ray, 1, 3), getWords(%ray, 4, 6), %size, %color, "", "", 1); //noUnclutterCheck is true
		%decal.color = %color;
		%this.lastFiber = $Sim::Time;
	}
}