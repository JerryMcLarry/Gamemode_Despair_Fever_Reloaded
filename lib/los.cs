function isWithinView(%a, %b, %vector, %angle)
{
	if (%angle $= "")
	{
		%angle = 90;
	}

	%product = vectorDot(%vector, vectorNormalize(vectorSub(%b, %a)));
	return %product >= 1 - (%angle / 360) * 2;
}

function Player::isWithinView(%this, %b, %angle)
{
	if (%angle $= "")
	{
		%client = %this.getControllingClient();

		if (isObject(%client))
		{
			%angle = %client.getControlCameraFOV();
		}
	}

	return isWithinView(%this.getEyePoint(), %b, %this.getEyeVector(), %angle);
}

//check if muzzle is obstructed by environment
function Player::getLOSPoint(%this, %pos, %vec, %mask, %dist)
{
	%endPoint = vectorAdd(%pos, vectorScale(%vec, %dist));
	%ray = containerRayCast(%pos, %endPoint, %mask, %this);

	if (%ray)
	{
		return getWords(%ray, 1, 3);
	}

	return %endPoint;
}