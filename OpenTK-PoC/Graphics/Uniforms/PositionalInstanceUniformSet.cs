using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal class PositionalInstanceUniformSet
{
    private readonly Matrix4Uniform _modelUniform;
    private readonly Matrix4Uniform _mvpUniform;
    private readonly Matrix4Uniform _normalUniform;
    // ich weiß nicht, warum die cameraPos aus CameraUniformSet nicht gesetzt wird
    private readonly Vector3Uniform _cameraPositionUniform;
    private readonly Matrix4Uniform _projectionUniform;
    private readonly Matrix4Uniform _viewUniform;

    private readonly Matrix4 _baseTransform;
    private readonly ICameraUniformData _cameraUniformData;
    private readonly bool _removeCameraTranslation;

    public PositionalInstanceUniformSet(Matrix4 baseTransform, ICameraUniformData cameraUniformData, bool removeCameraTranslation = false)
    {
        _baseTransform = baseTransform;
        _cameraUniformData = cameraUniformData;
        _removeCameraTranslation = removeCameraTranslation;
        _modelUniform = new Matrix4Uniform("modelMatrix");
        _mvpUniform = new Matrix4Uniform("mvpMatrix");
        _normalUniform = new Matrix4Uniform("normalMatrix");
        _cameraPositionUniform = new Vector3Uniform("cameraPos");
        _projectionUniform = new Matrix4Uniform("projectionMatrix");
        _viewUniform = new Matrix4Uniform("viewMatrix");
    }

    public IImmutableList<Uniform> GetUniformsFor(PositionalInstance instance)
    {
        var camera = _cameraUniformData.ViewMatrix;
        if (_removeCameraTranslation)
        {
            camera = camera.ClearTranslation();
        }

        _modelUniform.Matrix = _baseTransform * instance.ModelTransform;
        _mvpUniform.Matrix = _modelUniform.Matrix * camera * _cameraUniformData.ProjectionMatrix;
        
        var nMat = _modelUniform.Matrix.Inverted();
        nMat.Transpose();
        _normalUniform.Matrix = nMat;

        _cameraPositionUniform.Vector = _cameraUniformData.CameraPositionVector;
        _projectionUniform.Matrix = _cameraUniformData.ProjectionMatrix;
        _viewUniform.Matrix = _cameraUniformData.ViewMatrix;

        return ImmutableList.Create<Uniform>(_modelUniform, _mvpUniform, _normalUniform, _cameraPositionUniform, _projectionUniform, _viewUniform);
    }
}