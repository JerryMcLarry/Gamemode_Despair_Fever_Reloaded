function rotateVector(%vector, %yawDelta, %pitchDelta)
{
	%length = VectorLen(%vector);
	%vector = VectorNormalize(%vector);
	%x = getWord(%vector, 0);
	%y = getWord(%vector, 1);
	%z = getWord(%vector, 2);
	%yaw = mATan(%y, %x) + %yawDelta;
	%pitch = mATan(%z, mSqrt(%x * %x + %y * %y)) + %pitchDelta;
	%x = mCos(%pitch) * mCos(%yaw);
	%y = mCos(%pitch) * mSin(%yaw);
	%z = mSin(%pitch);
	return VectorScale(%x SPC %y SPC %z, %length);
}

function getRandomScalar(%magnitude)
{
	if(%magnitude $= "") %magnitude = 1;
	return (getRandom() * 2 - 1) * %magnitude;
}

function vectorSpread(%vector, %spread)
{
	%scalars = getRandomScalar() SPC getRandomScalar() SPC getRandomScalar();
	%scalars = vectorScale(%scalars, %spread);

	return matrixMulVector(matrixCreateFromEuler(%scalars), %vector);
}

function Player::getAimVector(%player)
{
	%fwd = %player.getForwardVector();
	%eye = %player.getEyeVector();
	
	%scale = vectorLen(setWord(%eye, 2, 0));

	return
		getWord(%fwd, 0) * %scale SPC
		getWord(%fwd, 1) * %scale SPC
		getWord(%eye, 2);
}

function commafy(%s)
{
	%i = %L = strlen(%s);
	while (%i-- >= 0)
		%o = (%i == 0 ? "" : ((%L - %i) % 3 ? "" : ",")) @ getSubStr(%s, %i, 1) @ %o;
	return %o;
}

datablock StaticShapeData(cubeShape)
{
    shapeFile = $Despair::Path @ "res/shapes/cube.dts";
};

datablock StaticShapeData(lineShape)
{
	shapeFile = $Despair::Path @ "res/shapes/line.dts";
};

datablock StaticShapeData(planeShape)
{
    shapeFile = $Despair::Path @ "res/shapes/plane.dts";
};

datablock StaticShapeData(planeCollisionShape)
{
    shapeFile = $Despair::Path @ "res/shapes/plane_collision.dts";
};

datablock StaticShapeData(planeGlowShape)
{
    shapeFile = $Despair::Path @ "res/shapes/plane_glow.dts";
};

datablock StaticShapeData(planeGlowCollisionShape)
{
    shapeFile = $Despair::Path @ "res/shapes/plane_glow_collision.dts";
};

if (!isObject(LinePool))
	new SimSet(LinePool);

if (!isObject(PrimitiveSet))
	new SimSet(PrimitiveSet);

function freeLine(%ref)
{
	if (isObject(%ref))
	{
		%ref.setScale("0 0 0");
		%ref.setTransform("0 0 -100");
		LinePool.add(%ref);
	}
	return 0;
}

function drawLine(%ref, %a, %b, %color)
{
	if (!isObject(%ref))
	{
		if (%count = LinePool.getCount())
		{
			%ref = LinePool.getObject(%count - 1);
			LinePool.remove(%ref);
		}
		else
		{
			%ref = new StaticShape()
			{
				datablock = lineShape;
			};
			
			PrimitiveSet.add(%ref);
		}
	}
	
	if (%ref.color !$= %color)
		%ref.setNodeColor("ALL", %ref.color = %color);
	
	%vector = VectorNormalize(VectorSub(%b, %a));
	%axis = VectorNormalize(VectorCross("0 1 0", %vector));
	%mag = mACos(VectorDot("0 1 0", %vector)) * -1;

	%ref.setTransform(VectorScale(VectorAdd(%a, %b), 0.5) SPC %axis SPC %mag);
	%ref.setScale("0.5 " @ VectorDist(%a, %b) @ " 0.5");

	return %ref;
}

function naturalGrammarList(%list, %b)
{
	%fields = getFieldCount(%list);
	if(%b $= "")
		%b = "and";
	if (%fields < 2) {
			return %list;
	}

	for (%i = 0; %i < %fields - 1; %i++) {
			%partial = %partial @ (%i ? ", " : "") @ getField(%list, %i);
	}

	return %partial SPC %b SPC getField(%list, %fields - 1);
}

function blendRGBA(%bg, %fg)
{
	%ba = getWord(%bg, 3);
	%fa = getWord(%fg, 3);

	%a = 1 - (1 - %fa) * (1 - %ba);

	%r = getWord(%fg, 0) * %fa / %a + getWord(%bg, 0) * %ba * (1 - %fa) / %a;
	%g = getWord(%fg, 1) * %fa / %a + getWord(%bg, 1) * %ba * (1 - %fa) / %a;
	%b = getWord(%fg, 2) * %fa / %a + getWord(%bg, 2) * %ba * (1 - %fa) / %a;

	return %r SPC %g SPC %b SPC %a;
}

function desaturateRGB(%rgb, %k)
{
	%r = getWord(%rgb, 0);
	%g = getWord(%rgb, 1);
	%b = getWord(%rgb, 2);

	%i = (%r + %g + %b) / 3;

	%r = %i * %k + %r * (1 - %k);
	%g = %i * %k + %g * (1 - %k);
	%b = %i * %k + %b * (1 - %k);

	return %r SPC %g SPC %b;
}

function addExtraResource(%name)
{
	if (MissionGroup.addedResource[%name]) return;
	MissionGroup.addedResource[%name] = "1";
	$EnvGuiServer::Resource[$EnvGuiServer::ResourceCount] = %name;
	$EnvGuiServer::ResourceCount++;
	setManifestDirty();
}

function findField(%fields, %f)
{
	for(%i = 0; %i < getFieldCount(%fields); %i++)
	{
		%a = getField(%fields, %i);
		if(%a $= %f)
			return %i;
	}
	return -1;
}

//Randomly pick a field
function pickField(%fields)
{
	return getField(%fields, getRandom(0, getFieldCount(%fields)-1));
}

function rgbToHex(%rgb)
{
	return
		rgbPartToHex(getWord(%rgb, 0)) @
		rgbPartToHex(getWord(%rgb, 1)) @
		rgbPartToHex(getWord(%rgb, 2))
	;
}

function rgbPartToHex(%color)
{
	%hex = "0123456789ABCDEF";

	%left = mFloor(%color / 16);
	%color -= %left * 16;

	return getSubStr(%hex, %left, 1) @ getSubStr(%hex, %color, 1);
}

function aOrAn(%string) 
{
	return strpos("aeiou", getSubStr(%string, 0, 1)) != -1 ? "an" : "a";
}