@script ExecuteInEditMode

var clipTexture : Texture;

function OnRenderObject() {
	Shader.SetGlobalTexture("_ClipTexture", clipTexture);

	var clipNear = -transform.localScale.x*0.5;
	var clipFar = transform.localScale.x*0.5;
	var scale = 1.0 / (clipFar-clipNear);
	var scaleOffset = Matrix4x4.TRS( Vector3(-clipNear,0,0), Quaternion.identity, Vector3(scale,scale,scale));
	Shader.SetGlobalMatrix("_ClipTextureMatrix", transform.worldToLocalMatrix * scaleOffset);
}
