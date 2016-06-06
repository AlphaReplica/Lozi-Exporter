var loziobject;
	
Lozi.load('examples/reflection/reflection_asset.js',function(obj)
{
	loziobject = obj.object;
	scene.add(loziobject);
	
	loziobject.setScale(3.5,3.5,3.5);
	
	onLoaded();
},onProgress);