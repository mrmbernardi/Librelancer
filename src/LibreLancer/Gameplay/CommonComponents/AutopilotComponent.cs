﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using System.Linq;
namespace LibreLancer
{
	public enum AutopilotBehaviours
	{
		None,
		Goto,
		Dock
	}
	public class AutopilotComponent : GameComponent
	{
		public AutopilotBehaviours CurrentBehaviour { get; private set; }
		public PIDController PitchControl = new PIDController();
		public PIDController YawControl = new PIDController();
        

		public AutopilotComponent(GameObject parent) : base(parent)
		{
			PitchControl.P = 4;
			YawControl.P = 4;
		}

		bool hasTriggeredAnimation = false;
		int lastTargetHp = 0;
        private string tlDockHP = null;
		void ResetDockState()
		{
			hasTriggeredAnimation = false;
			lastTargetHp = 0;
            tlDockHP = null;
        }
        
        
        private GameObject _targetObject;
        private Vector3 _targetPosition;
        private float _targetRadius;

        public bool CanCruise = false;
        public void GotoVec(Vector3 vec, bool cruise)
        {
            _targetObject = null;
            _targetPosition = vec;
            _targetRadius = 5;
            CurrentBehaviour = AutopilotBehaviours.Goto;
            CanCruise = cruise;
        }

        public void GotoObject(GameObject obj)
        {
            _targetObject = obj;
            CurrentBehaviour = AutopilotBehaviours.Goto;
            CanCruise = true;
        }

        public void Cancel()
        {
            _targetObject = null;
            CurrentBehaviour = AutopilotBehaviours.None;
        }
        

        public void StartDock(GameObject target)
        {
            _targetObject = target;
            var docking = target.GetComponent<CDockComponent>();
            if (docking != null)
            {
                if (docking.Action.Kind == DockKinds.Tradelane)
                {
                    var hpend = docking.GetDockHardpoints(Parent.PhysicsComponent.Body.Position).Last();
                    tlDockHP = hpend.Name;
                }
            }

            CurrentBehaviour = AutopilotBehaviours.Dock;
            CanCruise = true;
        }

        Vector3 GetTargetPoint()
        {
            if (_targetObject == null) return _targetPosition;
            return _targetObject.PhysicsComponent == null ? Vector3.Transform(Vector3.Zero,_targetObject.WorldTransform) : _targetObject.PhysicsComponent.Body.Position;
        }

        float GetTargetRadius()
        {
            if (_targetObject == null) return _targetRadius;
            return _targetObject.PhysicsComponent.Body.Collider.Radius;
        }
        
		public override void FixedUpdate(double time)
		{
			var control = Parent.GetComponent<ShipPhysicsComponent>();
            var input = Parent.GetComponent<ShipInputComponent>();
            if(input != null)input.AutopilotThrottle = 0;
            if (control == null) return;
			control.Pitch = control.Yaw = 0;
            if (CurrentBehaviour == AutopilotBehaviours.None)
			{
				ResetDockState();
				return;
			}
			Vector3 targetPoint = Vector3.Zero;
			float radius = -1;
			float maxSpeed = 1f;
			if (CurrentBehaviour == AutopilotBehaviours.Goto)
            {
                targetPoint = GetTargetPoint();
				ResetDockState();
			}
			else
			{
				var docking = _targetObject.GetComponent<CDockComponent>();
				if (docking == null)
				{
					CurrentBehaviour = AutopilotBehaviours.None;
					ResetDockState();
					Parent.World.BroadcastMessage(Parent, GameMessageKind.ManeuverFinished);
					return;
				}
                var hp = docking.GetDockHardpoints(Parent.PhysicsComponent.Body.Position).Skip(lastTargetHp).First();
				radius = 5;
                targetPoint = Vector3.Transform(Vector3.Zero, hp.Transform * _targetObject.WorldTransform);
				if (lastTargetHp > 0) maxSpeed = 0.3f;
				if (lastTargetHp == 2) radius = docking.TriggerRadius;
                if (docking.Action.Kind == DockKinds.Tradelane)
                {
                    if (docking.TryDockTL(Parent, tlDockHP))
                    {
                        CurrentBehaviour = AutopilotBehaviours.None;
                        ResetDockState();
                        //Parent.World.BroadcastMessage(Parent, GameMessageKind.ManeuverFinished);
                        return;
                    }
                }
				var d2 = (targetPoint - Parent.PhysicsComponent.Body.Position).Length();
				if (d2 < 80) maxSpeed = 0.3f;
			}

            float targetPower = 0;
            //Bring ship to within 40 metres of target
            var targetRadius = GetTargetRadius();
            var myRadius = Parent.PhysicsComponent.Body.Collider.Radius;
			var distance = (targetPoint - Parent.PhysicsComponent.Body.Position).Length();

            if (distance > 1000 && CanCruise) 
            {
                control.BeginCruise();
            }
            else if (distance < 600)
            {
                control.EndCruise();
            }
			var distrad = radius < 0 ? (targetRadius + myRadius + 40) : radius + myRadius;
			bool distanceSatisfied =  distrad >= distance;
			if (distanceSatisfied)
				targetPower = 0;
			else
				targetPower = maxSpeed;

			//Orientation
			var dt = time;
			var vec = Parent.InverseTransformPoint(targetPoint);
			//normalize it
			vec.Normalize();
			//
			bool directionSatisfied = (Math.Abs(vec.X) < 0.0015f && Math.Abs(vec.Y) < 0.0015f);
			if (!directionSatisfied)
			{
				control.Yaw = MathHelper.Clamp((float)YawControl.Update(0, vec.X, dt), -1, 1);
				control.Pitch = MathHelper.Clamp((float)PitchControl.Update(0, -vec.Y, dt), -1, 1);
			}
			else
			{
				control.Yaw = 0;
				control.Pitch = 0;
			}
			if (distanceSatisfied && directionSatisfied && CurrentBehaviour == AutopilotBehaviours.Goto)
			{
				Parent.World.BroadcastMessage(Parent, GameMessageKind.ManeuverFinished);
				CurrentBehaviour = AutopilotBehaviours.None;
			}
			if (distanceSatisfied && directionSatisfied && CurrentBehaviour == AutopilotBehaviours.Dock)
			{
				if (lastTargetHp < 2) lastTargetHp++;
				else
					targetPower = maxSpeed;
			}

            if(input != null)
             input.AutopilotThrottle = targetPower;
            control.EnginePower = targetPower;
        }

	}
}
