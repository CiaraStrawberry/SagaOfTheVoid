using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TrueSync.Physics2D;

internal static class WorldXmlSerializer
{
	private static XmlWriter _writer;

	private static void SerializeShape(Shape shape)
	{
		_writer.WriteStartElement("Shape");
		_writer.WriteAttributeString("Type", shape.ShapeType.ToString());
		_writer.WriteAttributeString("Density", shape.Density.ToString());
		switch (shape.ShapeType)
		{
		case ShapeType.Circle:
		{
			CircleShape circleShape = (CircleShape)shape;
			_writer.WriteElementString("Radius", circleShape.Radius.ToString());
			WriteElement("Position", circleShape.Position);
			break;
		}
		case ShapeType.Polygon:
		{
			PolygonShape polygonShape = (PolygonShape)shape;
			_writer.WriteStartElement("Vertices");
			foreach (TSVector2 vertex in polygonShape.Vertices)
			{
				WriteElement("Vertex", vertex);
			}
			_writer.WriteEndElement();
			WriteElement("Centroid", polygonShape.MassData.Centroid);
			break;
		}
		case ShapeType.Edge:
		{
			EdgeShape edgeShape = (EdgeShape)shape;
			WriteElement("Vertex1", edgeShape.Vertex1);
			WriteElement("Vertex2", edgeShape.Vertex2);
			break;
		}
		case ShapeType.Chain:
		{
			ChainShape chainShape = (ChainShape)shape;
			_writer.WriteStartElement("Vertices");
			foreach (TSVector2 vertex2 in chainShape.Vertices)
			{
				WriteElement("Vertex", vertex2);
			}
			_writer.WriteEndElement();
			WriteElement("NextVertex", chainShape.NextVertex);
			WriteElement("PrevVertex", chainShape.PrevVertex);
			break;
		}
		default:
			throw new Exception();
		}
		_writer.WriteEndElement();
	}

	private static void SerializeFixture(Fixture fixture)
	{
		_writer.WriteStartElement("Fixture");
		_writer.WriteAttributeString("Id", fixture.FixtureId.ToString());
		_writer.WriteStartElement("FilterData");
		_writer.WriteElementString("CategoryBits", ((int)fixture.CollisionCategories/*cast due to constrained. prefix*/).ToString());
		_writer.WriteElementString("MaskBits", ((int)fixture.CollidesWith/*cast due to constrained. prefix*/).ToString());
		_writer.WriteElementString("GroupIndex", fixture.CollisionGroup.ToString());
		_writer.WriteElementString("CollisionIgnores", Join("|", fixture._collisionIgnores));
		_writer.WriteEndElement();
		_writer.WriteElementString("Friction", fixture.Friction.ToString());
		_writer.WriteElementString("IsSensor", fixture.IsSensor.ToString());
		_writer.WriteElementString("Restitution", fixture.Restitution.ToString());
		if (fixture.UserData != null)
		{
			_writer.WriteStartElement("UserData");
			WriteDynamicType(fixture.UserData.GetType(), fixture.UserData);
			_writer.WriteEndElement();
		}
		_writer.WriteEndElement();
	}

	private static void SerializeBody(List<Fixture> fixtures, List<Shape> shapes, Body body)
	{
		_writer.WriteStartElement("Body");
		_writer.WriteAttributeString("Type", body.BodyType.ToString());
		_writer.WriteElementString("Active", body.Enabled.ToString());
		_writer.WriteElementString("AllowSleep", body.SleepingAllowed.ToString());
		_writer.WriteElementString("Angle", body.Rotation.ToString());
		_writer.WriteElementString("AngularDamping", body.AngularDamping.ToString());
		_writer.WriteElementString("AngularVelocity", body.AngularVelocity.ToString());
		_writer.WriteElementString("Awake", body.Awake.ToString());
		_writer.WriteElementString("Bullet", body.IsBullet.ToString());
		_writer.WriteElementString("FixedRotation", body.FixedRotation.ToString());
		_writer.WriteElementString("LinearDamping", body.LinearDamping.ToString());
		WriteElement("LinearVelocity", body.LinearVelocity);
		WriteElement("Position", body.Position);
		if (body.UserData != null)
		{
			_writer.WriteStartElement("UserData");
			WriteDynamicType(body.UserData.GetType(), body.UserData);
			_writer.WriteEndElement();
		}
		_writer.WriteStartElement("Bindings");
		for (int i = 0; i < body.FixtureList.Count; i++)
		{
			_writer.WriteStartElement("Pair");
			_writer.WriteAttributeString("FixtureId", FindIndex(fixtures, body.FixtureList[i]).ToString());
			_writer.WriteAttributeString("ShapeId", FindIndex(shapes, body.FixtureList[i].Shape).ToString());
			_writer.WriteEndElement();
		}
		_writer.WriteEndElement();
		_writer.WriteEndElement();
	}

	private static void SerializeJoint(List<Body> bodies, Joint2D joint)
	{
		_writer.WriteStartElement("Joint");
		_writer.WriteAttributeString("Type", joint.JointType.ToString());
		WriteElement("BodyA", FindIndex(bodies, joint.BodyA));
		WriteElement("BodyB", FindIndex(bodies, joint.BodyB));
		WriteElement("CollideConnected", joint.CollideConnected);
		WriteElement("Breakpoint", joint.Breakpoint);
		if (joint.UserData != null)
		{
			_writer.WriteStartElement("UserData");
			WriteDynamicType(joint.UserData.GetType(), joint.UserData);
			_writer.WriteEndElement();
		}
		switch (joint.JointType)
		{
		case JointType.Distance:
		{
			DistanceJoint distanceJoint = (DistanceJoint)joint;
			WriteElement("DampingRatio", distanceJoint.DampingRatio);
			WriteElement("FrequencyHz", distanceJoint.Frequency);
			WriteElement("Length", distanceJoint.Length);
			WriteElement("LocalAnchorA", distanceJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", distanceJoint.LocalAnchorB);
			break;
		}
		case JointType.Friction:
		{
			FrictionJoint frictionJoint = (FrictionJoint)joint;
			WriteElement("LocalAnchorA", frictionJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", frictionJoint.LocalAnchorB);
			WriteElement("MaxForce", frictionJoint.MaxForce);
			WriteElement("MaxTorque", frictionJoint.MaxTorque);
			break;
		}
		case JointType.Gear:
			throw new Exception("Gear joint not supported by serialization");
		case JointType.Wheel:
		{
			WheelJoint wheelJoint = (WheelJoint)joint;
			WriteElement("EnableMotor", wheelJoint.MotorEnabled);
			WriteElement("LocalAnchorA", wheelJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", wheelJoint.LocalAnchorB);
			WriteElement("MotorSpeed", wheelJoint.MotorSpeed);
			WriteElement("DampingRatio", wheelJoint.DampingRatio);
			WriteElement("MaxMotorTorque", wheelJoint.MaxMotorTorque);
			WriteElement("FrequencyHz", wheelJoint.Frequency);
			WriteElement("Axis", wheelJoint.Axis);
			break;
		}
		case JointType.Prismatic:
		{
			PrismaticJoint prismaticJoint = (PrismaticJoint)joint;
			WriteElement("EnableLimit", prismaticJoint.LimitEnabled);
			WriteElement("EnableMotor", prismaticJoint.MotorEnabled);
			WriteElement("LocalAnchorA", prismaticJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", prismaticJoint.LocalAnchorB);
			WriteElement("Axis", prismaticJoint.Axis);
			WriteElement("LowerTranslation", prismaticJoint.LowerLimit);
			WriteElement("UpperTranslation", prismaticJoint.UpperLimit);
			WriteElement("MaxMotorForce", prismaticJoint.MaxMotorForce);
			WriteElement("MotorSpeed", prismaticJoint.MotorSpeed);
			break;
		}
		case JointType.Pulley:
		{
			PulleyJoint pulleyJoint = (PulleyJoint)joint;
			WriteElement("WorldAnchorA", pulleyJoint.WorldAnchorA);
			WriteElement("WorldAnchorB", pulleyJoint.WorldAnchorB);
			WriteElement("LengthA", pulleyJoint.LengthA);
			WriteElement("LengthB", pulleyJoint.LengthB);
			WriteElement("LocalAnchorA", pulleyJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", pulleyJoint.LocalAnchorB);
			WriteElement("Ratio", pulleyJoint.Ratio);
			WriteElement("Constant", pulleyJoint.Constant);
			break;
		}
		case JointType.Revolute:
		{
			RevoluteJoint revoluteJoint = (RevoluteJoint)joint;
			WriteElement("EnableLimit", revoluteJoint.LimitEnabled);
			WriteElement("EnableMotor", revoluteJoint.MotorEnabled);
			WriteElement("LocalAnchorA", revoluteJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", revoluteJoint.LocalAnchorB);
			WriteElement("LowerAngle", revoluteJoint.LowerLimit);
			WriteElement("MaxMotorTorque", revoluteJoint.MaxMotorTorque);
			WriteElement("MotorSpeed", revoluteJoint.MotorSpeed);
			WriteElement("ReferenceAngle", revoluteJoint.ReferenceAngle);
			WriteElement("UpperAngle", revoluteJoint.UpperLimit);
			break;
		}
		case JointType.Weld:
		{
			WeldJoint weldJoint = (WeldJoint)joint;
			WriteElement("LocalAnchorA", weldJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", weldJoint.LocalAnchorB);
			break;
		}
		case JointType.Rope:
		{
			RopeJoint ropeJoint = (RopeJoint)joint;
			WriteElement("LocalAnchorA", ropeJoint.LocalAnchorA);
			WriteElement("LocalAnchorB", ropeJoint.LocalAnchorB);
			WriteElement("MaxLength", ropeJoint.MaxLength);
			break;
		}
		case JointType.Angle:
		{
			AngleJoint angleJoint = (AngleJoint)joint;
			WriteElement("BiasFactor", angleJoint.BiasFactor);
			WriteElement("MaxImpulse", angleJoint.MaxImpulse);
			WriteElement("Softness", angleJoint.Softness);
			WriteElement("TargetAngle", angleJoint.TargetAngle);
			break;
		}
		case JointType.Motor:
		{
			MotorJoint motorJoint = (MotorJoint)joint;
			WriteElement("AngularOffset", motorJoint.AngularOffset);
			WriteElement("LinearOffset", motorJoint.LinearOffset);
			WriteElement("MaxForce", motorJoint.MaxForce);
			WriteElement("MaxTorque", motorJoint.MaxTorque);
			WriteElement("CorrectionFactor", motorJoint.CorrectionFactor);
			break;
		}
		default:
			throw new Exception("Joint not supported");
		}
		_writer.WriteEndElement();
	}

	private static void WriteDynamicType(Type type, object val)
	{
		_writer.WriteElementString("Type", type.AssemblyQualifiedName);
		_writer.WriteStartElement("Value");
		XmlSerializer xmlSerializer = new XmlSerializer(type);
		XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
		xmlSerializerNamespaces.Add(string.Empty, string.Empty);
		xmlSerializer.Serialize(_writer, val, xmlSerializerNamespaces);
		_writer.WriteEndElement();
	}

	private static void WriteElement(string name, TSVector2 vec)
	{
		_writer.WriteElementString(name, string.Concat(vec.x, " ", vec.y));
	}

	private static void WriteElement(string name, int val)
	{
		_writer.WriteElementString(name, val.ToString());
	}

	private static void WriteElement(string name, bool val)
	{
		_writer.WriteElementString(name, val.ToString());
	}

	private static void WriteElement(string name, FP val)
	{
		_writer.WriteElementString(name, val.ToString());
	}

	private static int FindIndex(List<Body> list, Body item)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == item)
			{
				return i;
			}
		}
		return -1;
	}

	private static int FindIndex(List<Fixture> list, Fixture item)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].CompareTo(item))
			{
				return i;
			}
		}
		return -1;
	}

	private static int FindIndex(List<Shape> list, Shape item)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].CompareTo(item))
			{
				return i;
			}
		}
		return -1;
	}

	private static string Join<T>(string separator, IEnumerable<T> values)
	{
		using IEnumerator<T> enumerator = values.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (enumerator.Current != null)
		{
			string text = enumerator.Current.ToString();
			if (text != null)
			{
				stringBuilder.Append(text);
			}
		}
		while (enumerator.MoveNext())
		{
			stringBuilder.Append(separator);
			if (enumerator.Current != null)
			{
				string text2 = enumerator.Current.ToString();
				if (text2 != null)
				{
					stringBuilder.Append(text2);
				}
			}
		}
		return stringBuilder.ToString();
	}

	internal static void Serialize(World world, Stream stream)
	{
		List<Body> list = new List<Body>();
		List<Fixture> list2 = new List<Fixture>();
		List<Shape> list3 = new List<Shape>();
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Indent = true;
		xmlWriterSettings.NewLineOnAttributes = false;
		xmlWriterSettings.OmitXmlDeclaration = true;
		_writer = XmlWriter.Create(stream, xmlWriterSettings);
		_writer.WriteStartElement("World");
		_writer.WriteAttributeString("Version", "3");
		WriteElement("Gravity", world.Gravity);
		_writer.WriteStartElement("Shapes");
		foreach (Body body in world.BodyList)
		{
			foreach (Fixture fixture in body.FixtureList)
			{
				if (!list3.Any((Shape s2) => fixture.Shape.CompareTo(s2)))
				{
					SerializeShape(fixture.Shape);
					list3.Add(fixture.Shape);
				}
			}
		}
		_writer.WriteEndElement();
		_writer.WriteStartElement("Fixtures");
		foreach (Body body2 in world.BodyList)
		{
			foreach (Fixture fixture2 in body2.FixtureList)
			{
				if (!list2.Any((Fixture f2) => fixture2.CompareTo(f2)))
				{
					SerializeFixture(fixture2);
					list2.Add(fixture2);
				}
			}
		}
		_writer.WriteEndElement();
		_writer.WriteStartElement("Bodies");
		foreach (Body body3 in world.BodyList)
		{
			list.Add(body3);
			SerializeBody(list2, list3, body3);
		}
		_writer.WriteEndElement();
		_writer.WriteStartElement("Joints");
		foreach (Joint2D joint in world.JointList)
		{
			SerializeJoint(list, joint);
		}
		_writer.WriteEndElement();
		_writer.WriteEndElement();
		_writer.Flush();
		_writer.Close();
	}
}
