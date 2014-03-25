// Copyright (c) Arjen Post. See License.txt in the project root for license information. Credits go to Marc Gravell 
// for the original idea, which found here https://code.google.com/p/fast-member/, and some parts of the code.

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace Gonzales.Test
{
    [ExcludeFromCodeCoverage]
    public class TypeAccessorTests
    {
        [TestClass]
        public class TheTryGetMethod
        {
            [TestMethod]
            public void ShouldReadMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 123;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                object a;

                //Act
                var result = accessor.TryGetValue(@dynamic, "A", out a);

                // Assert
                Assert.AreEqual(123, a);
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldNotReadNonExistingMemberOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                object a;

                //Act
                var result = accessor.TryGetValue(@dynamic, "nonexisting", out a);

                // Assert
                Assert.IsNull(a);
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ShouldReadPropertiesOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class
                {
                    A = 123,
                    B = "abc",
                    C = now,
                    D = null
                };

                var accessor = TypeAccessor.Create(typeof(Class));

                object a;
                object b;
                object c;
                object d;

                var result = true;

                //Act
                result &= accessor.TryGetValue(@class, "A", out a);
                result &= accessor.TryGetValue(@class, "B", out b);
                result &= accessor.TryGetValue(@class, "C", out c);
                result &= accessor.TryGetValue(@class, "D", out d);

                // Assert
                Assert.AreEqual(123, a);
                Assert.AreEqual("abc", b);
                Assert.AreEqual(now, c);
                Assert.AreEqual(null, d);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldReadFieldsOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class
                {
                    E = 123,
                    F = "abc",
                    G = now,
                    H = null
                };

                var accessor = TypeAccessor.Create(typeof(Class));

                object e;
                object f;
                object g;
                object h;

                var result = true;

                //Act
                result &= accessor.TryGetValue(@class, "E", out e);
                result &= accessor.TryGetValue(@class, "F", out f);
                result &= accessor.TryGetValue(@class, "G", out g);
                result &= accessor.TryGetValue(@class, "H", out h);

                // Assert
                Assert.AreEqual(123, e);
                Assert.AreEqual("abc", f);
                Assert.AreEqual(now, g);
                Assert.AreEqual(null, h);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldNotReadNonExistingMemberOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                object a;

                //Act
                var result = accessor.TryGetValue(@class, "nonexisting", out a);

                // Assert
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ShouldReadPropertiesOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct
                {
                    A = 123,
                    B = "abc",
                    C = now,
                    D = null
                };

                var accessor = TypeAccessor.Create(typeof(Struct));

                object a;
                object b;
                object c;
                object d;

                var result = true;

                //Act
                result &= accessor.TryGetValue(@struct, "A", out a);
                result &= accessor.TryGetValue(@struct, "B", out b);
                result &= accessor.TryGetValue(@struct, "C", out c);
                result &= accessor.TryGetValue(@struct, "D", out d);

                // Assert
                Assert.AreEqual(123, a);
                Assert.AreEqual("abc", b);
                Assert.AreEqual(now, c);
                Assert.AreEqual(null, d);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldReadFieldsOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct
                {
                    E = 123,
                    F = "abc",
                    G = now,
                    H = null
                };

                var accessor = TypeAccessor.Create(typeof(Struct));

                object e;
                object f;
                object g;
                object h;

                var result = true;

                //Act
                result &= accessor.TryGetValue(@struct, "E", out e);
                result &= accessor.TryGetValue(@struct, "F", out f);
                result &= accessor.TryGetValue(@struct, "G", out g);
                result &= accessor.TryGetValue(@struct, "H", out h);

                // Assert
                Assert.AreEqual(123, e);
                Assert.AreEqual("abc", f);
                Assert.AreEqual(now, g);
                Assert.AreEqual(null, h);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldNotReadNonExistingMemberOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                object a;

                //Act
                var result = accessor.TryGetValue(@struct, "nonexisting", out a);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [TestClass]
        public class TheGetMethod
        {
            [TestMethod]
            public void ShouldReadMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 123;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                var a = accessor.GetValue(@dynamic, "A");

                // Assert
                Assert.AreEqual(123, a);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadNonExistingMemberOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                accessor.GetValue(@dynamic, "nonexisting");
            }

            [TestMethod]
            public void ShouldReadPropertiesOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class
                {
                    A = 123,
                    B = "abc",
                    C = now,
                    D = null
                };

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                var a = accessor.GetValue(@class, "A");
                var b = accessor.GetValue(@class, "B");
                var c = accessor.GetValue(@class, "C");
                var d = accessor.GetValue(@class, "D");

                // Assert
                Assert.AreEqual(123, a);
                Assert.AreEqual("abc", b);
                Assert.AreEqual(now, c);
                Assert.AreEqual(null, d);
            }

            [TestMethod]
            public void ShouldReadFieldsOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class
                {
                    E = 123,
                    F = "abc",
                    G = now,
                    H = null
                };

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                var e = accessor.GetValue(@class, "E");
                var f = accessor.GetValue(@class, "F");
                var g = accessor.GetValue(@class, "G");
                var h = accessor.GetValue(@class, "H");

                // Assert
                Assert.AreEqual(123, e);
                Assert.AreEqual("abc", f);
                Assert.AreEqual(now, g);
                Assert.AreEqual(null, h);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadNonExistingMemberOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.GetValue(@class, "nonexisting");
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadPrivatePropertyOnClass() // For now...
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.GetValue(@class, "I");
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadPropertyWithPrivateGetterOnClass() // For now...
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.GetValue(@class, "K");
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadPrivateFieldOnClass() // For now...
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.GetValue(@class, "L");
            }

            [TestMethod]
            public void ShouldReadPropertiesOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct
                {
                    A = 123,
                    B = "abc",
                    C = now,
                    D = null
                };

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                var a = accessor.GetValue(@struct, "A");
                var b = accessor.GetValue(@struct, "B");
                var c = accessor.GetValue(@struct, "C");
                var d = accessor.GetValue(@struct, "D");

                // Assert
                Assert.AreEqual(123, a);
                Assert.AreEqual("abc", b);
                Assert.AreEqual(now, c);
                Assert.AreEqual(null, d);
            }

            [TestMethod]
            public void ShouldReadFieldsOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct
                {
                    E = 123,
                    F = "abc",
                    G = now,
                    H = null
                };

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                var e = accessor.GetValue(@struct, "E");
                var f = accessor.GetValue(@struct, "F");
                var g = accessor.GetValue(@struct, "G");
                var h = accessor.GetValue(@struct, "H");

                // Assert
                Assert.AreEqual(123, e);
                Assert.AreEqual("abc", f);
                Assert.AreEqual(now, g);
                Assert.AreEqual(null, h);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadNonExistingMemberOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                accessor.GetValue(@struct, "nonexisting");
            }

            [TestMethod, ExpectedException(typeof(RuntimeBinderException))]
            public void ShouldOnNonExistingMemberOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = TypeAccessor.Create(@dynamic.GetType(), TypeAccessorOptions.DisableArgumentValidation);

                //Act
                var a = accessor.GetValue(@dynamic, "A");

                // Assert
                Assert.AreEqual(123, a);
            }

            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullMemberNameOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 123;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                var a = accessor.GetValue(@dynamic, null);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnEmptyMemberNameOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 123;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                var a = accessor.GetValue(@dynamic, "");
            }
        }

        [TestClass]
        public class TheIndexerGetMethod
        {
            [TestMethod]
            public void ShouldReadMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 123;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                var a = accessor[@dynamic, "A"];

                // Assert
                Assert.AreEqual(123, a);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadNonExistingMemberOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                var a = accessor[@dynamic, "nonexisting"];
            }

            [TestMethod]
            public void ShouldReadPropertiesOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class
                {
                    A = 123,
                    B = "abc",
                    C = now,
                    D = null
                };

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                var a = accessor[@class, "A"];
                var b = accessor[@class, "B"];
                var c = accessor[@class, "C"];
                var d = accessor[@class, "D"];

                // Assert
                Assert.AreEqual(123, a);
                Assert.AreEqual("abc", b);
                Assert.AreEqual(now, c);
                Assert.AreEqual(null, d);
            }

            [TestMethod]
            public void ShouldReadFieldsOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class
                {
                    E = 123,
                    F = "abc",
                    G = now,
                    H = null
                };

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                var e = accessor[@class, "E"];
                var f = accessor[@class, "F"];
                var g = accessor[@class, "G"];
                var h = accessor[@class, "H"];

                // Assert
                Assert.AreEqual(123, e);
                Assert.AreEqual("abc", f);
                Assert.AreEqual(now, g);
                Assert.AreEqual(null, h);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadNonExistingMemberOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                var a = accessor[@class, "nonexisting"];
            }

            [TestMethod]
            public void ShouldReadPropertiesOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct
                {
                    A = 123,
                    B = "abc",
                    C = now,
                    D = null
                };

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                var a = accessor[@struct, "A"];
                var b = accessor[@struct, "B"];
                var c = accessor[@struct, "C"];
                var d = accessor[@struct, "D"];

                // Assert
                Assert.AreEqual(123, a);
                Assert.AreEqual("abc", b);
                Assert.AreEqual(now, c);
                Assert.AreEqual(null, d);
            }

            [TestMethod]
            public void ShouldReadFieldsOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct
                {
                    E = 123,
                    F = "abc",
                    G = now,
                    H = null
                };

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                var e = accessor[@struct, "E"];
                var f = accessor[@struct, "F"];
                var g = accessor[@struct, "G"];
                var h = accessor[@struct, "H"];

                // Assert
                Assert.AreEqual(123, e);
                Assert.AreEqual("abc", f);
                Assert.AreEqual(now, g);
                Assert.AreEqual(null, h);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnReadNonExistingMemberOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                var a = accessor[@struct, "nonexisting"];
            }
        }

        [TestClass]
        public class TheTrySetMethod
        {
            [TestMethod]
            public void ShouldWriteMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 456;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                var result = accessor.TrySetValue(@dynamic, "A", 123);

                // Assert
                Assert.AreEqual(123, @dynamic.A);
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldWriteNonExistingMemberOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                var result = accessor.TrySetValue(@dynamic, "A", 123);

                // Assert
                Assert.AreEqual(123, @dynamic.A);
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldWritePropertiesOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                var result = true;

                //Act
                result &= accessor.TrySetValue(@class, "A", 123);
                result &= accessor.TrySetValue(@class, "B", "abc");
                result &= accessor.TrySetValue(@class, "C", now);
                result &= accessor.TrySetValue(@class, "D", null);

                // Assert
                Assert.AreEqual(123, @class.A);
                Assert.AreEqual("abc", @class.B);
                Assert.AreEqual(now, @class.C);
                Assert.AreEqual(null, @class.D);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldWriteFieldsOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                var result = true;

                //Act
                result &= accessor.TrySetValue(@class, "E", 123);
                result &= accessor.TrySetValue(@class, "F", "abc");
                result &= accessor.TrySetValue(@class, "G", now);
                result &= accessor.TrySetValue(@class, "H", null);

                // Assert
                Assert.AreEqual(123, @class.E);
                Assert.AreEqual("abc", @class.F);
                Assert.AreEqual(now, @class.G);
                Assert.AreEqual(null, @class.H);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldNotWriteNonExistingMemberOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                var result = accessor.TrySetValue(@class, "nonexisting", 123);

                // Assert
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ShouldWritePropertiesOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                var result = true;

                //Act
                result &= accessor.TrySetValue(@struct, "A", 123);
                result &= accessor.TrySetValue(@struct, "B", "abc");
                result &= accessor.TrySetValue(@struct, "C", now);
                result &= accessor.TrySetValue(@struct, "D", null);

                // Assert
                Assert.AreEqual(123, @struct.A);
                Assert.AreEqual("abc", @struct.B);
                Assert.AreEqual(now, @struct.C);
                Assert.AreEqual(null, @struct.D);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldWriteFieldsOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                var result = true;

                //Act
                result &= accessor.TrySetValue(@struct, "E", 123);
                result &= accessor.TrySetValue(@struct, "F", "abc");
                result &= accessor.TrySetValue(@struct, "G", now);
                result &= accessor.TrySetValue(@struct, "H", null);

                // Assert
                Assert.AreEqual(123, @struct.E);
                Assert.AreEqual("abc", @struct.F);
                Assert.AreEqual(now, @struct.G);
                Assert.AreEqual(null, @struct.H);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldNotWriteNonExistingMemberOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                var result = accessor.TrySetValue(@struct, "nonexisting", 123);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [TestClass]
        public class TheSetMethod
        {
            [TestMethod]
            public void ShouldWriteMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 456;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                accessor.SetValue(@dynamic, "A", 123);

                // Assert
                Assert.AreEqual(123, @dynamic.A);
            }

            [TestMethod]
            public void ShouldWriteNonExistingMemberOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                accessor.SetValue(@dynamic, "A", 123);

                // Assert
                Assert.AreEqual(123, @dynamic.A);
            }

            [TestMethod]
            public void ShouldWritePropertiesOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.SetValue(@class, "A", 123);
                accessor.SetValue(@class, "B", "abc");
                accessor.SetValue(@class, "C", now);
                accessor.SetValue(@class, "D", null);

                // Assert
                Assert.AreEqual(123, @class.A);
                Assert.AreEqual("abc", @class.B);
                Assert.AreEqual(now, @class.C);
                Assert.AreEqual(null, @class.D);
            }

            [TestMethod]
            public void ShouldWriteFieldsOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                var result = true;

                //Act
                accessor.SetValue(@class, "E", 123);
                accessor.SetValue(@class, "F", "abc");
                accessor.SetValue(@class, "G", now);
                accessor.SetValue(@class, "H", null);

                // Assert
                Assert.AreEqual(123, @class.E);
                Assert.AreEqual("abc", @class.F);
                Assert.AreEqual(now, @class.G);
                Assert.AreEqual(null, @class.H);

                Assert.IsTrue(result);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnWriteNonExistingMemberOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.SetValue(@class, "nonexisting", 123);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnWritePrivatePropertyOnClass() // For now...
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.SetValue(@class, "I", 123);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnWritePropertyWithPrivateSetterOnClass() // For now...
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.SetValue(@class, "J", 123);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnWritePrivateFieldOnClass() // For now...
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor.SetValue(@class, "L", 123);
            }

            [TestMethod]
            public void ShouldWritePropertiesOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                accessor.SetValue(@struct, "A", 123);
                accessor.SetValue(@struct, "B", "abc");
                accessor.SetValue(@struct, "C", now);
                accessor.SetValue(@struct, "D", null);

                // Assert
                Assert.AreEqual(123, @struct.A);
                Assert.AreEqual("abc", @struct.B);
                Assert.AreEqual(now, @struct.C);
                Assert.AreEqual(null, @struct.D);
            }

            [TestMethod]
            public void ShouldWriteFieldsOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                var result = true;

                //Act
                accessor.SetValue(@struct, "E", 123);
                accessor.SetValue(@struct, "F", "abc");
                accessor.SetValue(@struct, "G", now);
                accessor.SetValue(@struct, "H", null);

                // Assert
                Assert.AreEqual(123, @struct.E);
                Assert.AreEqual("abc", @struct.F);
                Assert.AreEqual(now, @struct.G);
                Assert.AreEqual(null, @struct.H);

                Assert.IsTrue(result);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnWriteNonExistingMemberOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                accessor.SetValue(@struct, "nonexisting", 123);
            }
        }

        [TestClass]
        public class TheIndexerSetMethod
        {
            [TestMethod]
            public void ShouldWriteMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                @dynamic.A = 456;

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                accessor[@dynamic, "A"] = 123;

                // Assert
                Assert.AreEqual(123, @dynamic.A);
            }

            [TestMethod]
            public void ShouldWriteNonExistingMemberOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                accessor[@dynamic, "A"] = 123;

                // Assert
                Assert.AreEqual(123, @dynamic.A);
            }

            [TestMethod]
            public void ShouldWritePropertiesOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor[@class, "A"] = 123;
                accessor[@class, "B"] = "abc";
                accessor[@class, "C"] = now;
                accessor[@class, "D"] = null;

                // Assert
                Assert.AreEqual(123, @class.A);
                Assert.AreEqual("abc", @class.B);
                Assert.AreEqual(now, @class.C);
                Assert.AreEqual(null, @class.D);
            }

            [TestMethod]
            public void ShouldWriteFieldsOnClass()
            {
                // Arrange
                var now = DateTime.Now;

                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                var result = true;

                //Act
                accessor[@class, "E"] = 123;
                accessor[@class, "F"] = "abc";
                accessor[@class, "G"] = now;
                accessor[@class, "H"] = null;

                // Assert
                Assert.AreEqual(123, @class.E);
                Assert.AreEqual("abc", @class.F);
                Assert.AreEqual(now, @class.G);
                Assert.AreEqual(null, @class.H);

                Assert.IsTrue(result);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnWriteNonExistingMemberOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor[@class, "nonexisting"] = 123;
            }

            [TestMethod]
            public void ShouldWritePropertiesOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                accessor[@struct, "A"] = 123;
                accessor[@struct, "B"] = "abc";
                accessor[@struct, "C"] = now;
                accessor[@struct, "D"] = null;

                // Assert
                Assert.AreEqual(123, @struct.A);
                Assert.AreEqual("abc", @struct.B);
                Assert.AreEqual(now, @struct.C);
                Assert.AreEqual(null, @struct.D);
            }

            [TestMethod]
            public void ShouldWriteFieldsOnStruct()
            {
                // Arrange
                var now = DateTime.Now;

                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                var result = true;

                //Act
                accessor[@struct, "E"] = 123;
                accessor[@struct, "F"] = "abc";
                accessor[@struct, "G"] = now;
                accessor[@struct, "H"] = null;

                // Assert
                Assert.AreEqual(123, @struct.E);
                Assert.AreEqual("abc", @struct.F);
                Assert.AreEqual(now, @struct.G);
                Assert.AreEqual(null, @struct.H);

                Assert.IsTrue(result);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnWriteNonExistingMemberOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Struct));

                //Act
                accessor[@struct, "nonexisting"] = 123;
            }
        }

        [TestClass]
        public class TheValidateObjectMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullObject()
            {
                // Arrange
                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor[null, "A"] = 123;
            }

            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullObjectDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();
                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                accessor[null, "A"] = 123;
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnInvalidType()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Class));

                //Act
                accessor[@struct, "A"] = 123;
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnInvalidTypeDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(@dynamic.GetType());

                //Act
                accessor[@struct, "A"] = 123;
            }

            [TestMethod, ExpectedException(typeof(NullReferenceException))]
            public void ShouldThrowOnNullObjectWithoutInputValidation()
            {
                // Arrange
                var accessor = TypeAccessor.Create(typeof(Class), TypeAccessorOptions.DisableArgumentValidation);

                //Act
                accessor[null, "A"] = 123;
            }

            [TestMethod, ExpectedException(typeof(InvalidCastException))]
            public void ShouldThrowOnInvalidTypeWithoutInputValidation()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = TypeAccessor.Create(typeof(Class), TypeAccessorOptions.DisableArgumentValidation);

                //Act
                accessor[@struct, "A"] = 123;
            }
        }

        [TestClass]
        public class TheCreateMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullType()
            {
                // Act
                TypeAccessor.Create(null);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnNonPublicType()
            {
                // Act
                TypeAccessor.Create(typeof(InternalClass));
            }
        }
    }
}