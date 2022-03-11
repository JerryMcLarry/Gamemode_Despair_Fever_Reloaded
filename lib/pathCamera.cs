datablock PathCameraData(DefaultPathCamera)
{
	isPathCamera = 1;
};

function GameConnection::playPath(%this, %path)
{
	if (!isObject(%path) || !%path.getCount())
	{
		return;
	}

	if (!isObject(%this.pathCamera))
	{
		%this.pathCamera = new PathCamera()
		{
			datablock = DefaultPathCamera;
		};

		MissionCleanup.add(%this.pathCamera);

		%this.pathCamera.setState("stop");
		%this.pathCamera.scopeToClient(%this);
	}

	%this.pathCamera.setPosition(0);
	%this.pathCamera.reset();

	for (%i = 0; %i < %path.getCount(); %i++)
	{
		%marker = %path.getObject(%i);

		%this.pathCamera.pushBack(
			%marker.getTransform(),
			%marker.speed,
			%marker.type,
			%marker.smoothingType
		);
		if (!%i)
		{
			%this.pathCamera.setPosition(1);
			%this.pathCamera.popFront();
		}
	}

	%this.setControlObject(%this.pathCamera);

	%this.pathCamera.setTarget(%path.getCount());
	%this.pathCamera.setState("forward");
}

function Path::addFromCamera(%this, %camera, %speed, %type, %smoothingType)
{
	if (%speed $= "")
	{
		%speed = 6;
	}

	if (%type $= "")
	{
		%type = "Normal";
	}

	if (%smoothingType $= "")
	{
		%smoothingType = "Linear";
	}

	%obj = new Marker()
	{
		speed = %speed;
		type = %type;
		smoothingType = %smoothingType;
	};

	MissionCleanup.add(%obj);
	%obj.setTransform(%camera.getTransform());

	%this.add(%obj);
}