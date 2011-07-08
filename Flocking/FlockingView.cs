/*
 * Copyright (c) Contributors, https://github.com/jonc/osboids
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Region.Framework.Scenes;

namespace Flocking
{
	public class FlockingView
	{
		private const float DEG_TO_RAD = 0.01745329238f;
		
		private Scene m_scene;
		private UUID m_owner;
		
		private Dictionary<string, SceneObjectGroup> m_sogMap = new Dictionary<string, SceneObjectGroup> ();
		
		public FlockingView (Scene scene)
		{
			m_scene = scene;	
		}
		
		public void PostInitialize ()
		{
			m_owner = m_scene.RegionInfo.EstateSettings.EstateOwner;
		}

		public void Render (List<Boid> boids)
		{
			foreach (Boid boid in boids) {
				SceneObjectGroup sog = DrawBoid (boid);
				//sog.ScheduleGroupForTerseUpdate ();
			}
		}
		
		private SceneObjectGroup DrawBoid (Boid boid)
		{
			SceneObjectPart existing = m_scene.GetSceneObjectPart (boid.Id);


			SceneObjectGroup copy;
			if (existing == null) {
				SceneObjectGroup group = findByName ("boidPrim");
				copy = CopyPrim (group, boid.Id);
				m_sogMap [boid.Id] = copy;
				m_scene.AddNewSceneObject (copy, false);
			} else {
				copy = existing.ParentGroup;
			}
			
			Quaternion rotation = CalcRotationToEndpoint (copy, copy.AbsolutePosition, boid.Location);
			copy.UpdateGroupRotationPR (boid.Location, rotation);
			return copy;
		}
		
		private static Quaternion CalcRotationToEndpoint (SceneObjectGroup copy, Vector3 sv, Vector3 ev)
		{
			Vector3 currDirVec = Vector3.UnitX;
			float angle = 0f;

			copy.GroupRotation.GetAxisAngle (out currDirVec, out angle);
			currDirVec.Normalize ();


			Vector3 desiredDirVec = Vector3.Subtract (ev, sv);
			desiredDirVec.Normalize ();

			Quaternion rot = Vector3.RotationBetween (currDirVec, desiredDirVec);

			//Quaternion x90 = Quaternion.CreateFromEulers (90f * DEG_TO_RAD, 0f, 0f);
			//rot = rot * x90;
			
			return rot;
		}
		
		private SceneObjectGroup CopyPrim (SceneObjectGroup prim, string name)
		{
			SceneObjectGroup copy = prim.Copy (true);
			copy.Name = name;
			copy.DetachFromBackup ();
			return copy;
		}
		
		private SceneObjectGroup findByName (string name)
		{
			SceneObjectGroup retVal = null;
			foreach (EntityBase e in m_scene.GetEntities()) {
				if (e.Name == name) {
					retVal = (SceneObjectGroup)e;
					break;
				}
			}
			
			// can't find it so make a default one
			if (retVal == null) {
				retVal = MakeDefaultPrim (name);
			}

			return retVal;
		}

		private SceneObjectGroup MakeDefaultPrim (string name)
		{
			PrimitiveBaseShape shape = PrimitiveBaseShape.CreateSphere ();
			shape.Scale = new Vector3 (0.5f, 0.5f, 2.5f);

			SceneObjectGroup prim = new SceneObjectGroup (m_owner, new Vector3 (128f, 128f, 25f), shape);
			prim.Name = name;
			prim.DetachFromBackup ();
			m_scene.AddNewSceneObject (prim, false);

			return prim;
		}



	}
}

