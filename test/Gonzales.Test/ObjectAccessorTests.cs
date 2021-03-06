﻿// Copyright (c) Arjen Post. See License.txt in the project root for license information. Credits go to Marc Gravell 
// for the original idea, which found here https://code.google.com/p/fast-member/, and some parts of the code.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;

namespace Gonzales.Test
{
    [ExcludeFromCodeCoverage]
    public class ObjectAccessorTests
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

                var accessor = ObjectAccessor.Create(@dynamic);

                object a;

                //Act
                var result = accessor.TryGetValue("A", out a);

                // Assert
                Assert.AreEqual(123, a);
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldReadMembersOnClass()
            {
                // Arrange
                var @class = new Class()
                {
                    A = 123
                };

                var accessor = ObjectAccessor.Create(@class);

                object a;

                //Act
                var result = accessor.TryGetValue("A", out a);

                // Assert
                Assert.AreEqual(123, a);
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldReadMembersOnStruct()
            {
                // Arrange
                var @struct = new Struct()
                {
                    A = 123
                };

                var accessor = ObjectAccessor.Create(@struct);

                object a;

                //Act
                var result = accessor.TryGetValue("A", out a);

                // Assert
                Assert.AreEqual(123, a);
                Assert.IsTrue(result);
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

                var accessor = ObjectAccessor.Create(@dynamic);

                //Act
                var a = accessor.GetValue("A");

                // Assert
                Assert.AreEqual(123, a);
            }

            [TestMethod]
            public void ShouldReadMembersOnClass()
            {
                // Arrange
                var @class = new Class()
                {
                    A = 123
                };

                var accessor = ObjectAccessor.Create(@class);

                //Act
                var a = accessor.GetValue("A");

                // Assert
                Assert.AreEqual(123, a);
            }

            [TestMethod]
            public void ShouldReadMembersOnStruct()
            {
                // Arrange
                var @struct = new Struct()
                {
                    A = 123
                };

                var accessor = ObjectAccessor.Create(@struct);

                //Act
                var a = accessor.GetValue("A");

                // Assert
                Assert.AreEqual(123, a);
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

                var accessor = ObjectAccessor.Create(@dynamic);

                //Act
                var a = accessor["A"];

                // Assert
                Assert.AreEqual(123, a);
            }

            [TestMethod]
            public void ShouldReadMembersOnClass()
            {
                // Arrange
                var @class = new Class()
                {
                    A = 123
                };

                var accessor = ObjectAccessor.Create(@class);

                //Act
                var a = accessor["A"];

                // Assert
                Assert.AreEqual(123, a);
            }

            [TestMethod]
            public void ShouldReadMembersOnStruct()
            {
                // Arrange
                var @struct = new Struct()
                {
                    A = 123
                };

                var accessor = ObjectAccessor.Create(@struct);

                //Act
                var a = accessor["A"];

                // Assert
                Assert.AreEqual(123, a);
            }
        }

        [TestClass]
        public class TheTrySetMethod
        {
            [TestMethod]
            public void ShouldReadMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = ObjectAccessor.Create(@dynamic);

                //Act
                var result = accessor.TrySetValue("A", 123);

                // Assert
                Assert.AreEqual(123, @dynamic.A);
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldReadMembersOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = ObjectAccessor.Create(@class);

                //Act
                var result = accessor.TrySetValue("A", 123);

                // Assert
                Assert.AreEqual(123, @class.A);
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ShouldReadMembersOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = ObjectAccessor.Create(@struct);

                //Act
                var result = accessor.TrySetValue("A", 123);

                // Assert
                Assert.AreEqual(123, @struct.A);
                Assert.IsTrue(result);
            }
        }

        [TestClass]
        public class TheSetMethod
        {
            [TestMethod]
            public void ShouldReadMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = ObjectAccessor.Create(@dynamic);

                //Act
                accessor.SetValue("A", 123);

                // Assert
                Assert.AreEqual(123, @dynamic.A);
            }

            [TestMethod]
            public void ShouldReadMembersOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = ObjectAccessor.Create(@class);

                //Act
                accessor.SetValue("A", 123);

                // Assert
                Assert.AreEqual(123, @class.A);
            }

            [TestMethod]
            public void ShouldReadMembersOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = ObjectAccessor.Create(@struct);

                //Act
                accessor.SetValue("A", 123);

                // Assert
                Assert.AreEqual(123, @struct.A);
            }
        }

        [TestClass]
        public class TheIndexerSetMethod
        {
            [TestMethod]
            public void ShouldReadMembersOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = ObjectAccessor.Create(@dynamic);

                //Act
                accessor["A"] = 123;

                // Assert
                Assert.AreEqual(123, @dynamic.A);
            }

            [TestMethod]
            public void ShouldReadMembersOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = ObjectAccessor.Create(@class);

                //Act
                accessor["A"] = 123;

                // Assert
                Assert.AreEqual(123, @class.A);
            }

            [TestMethod]
            public void ShouldReadMembersOnStruct()
            {
                // Arrange
                var @struct = new Struct();

                var accessor = ObjectAccessor.Create(@struct);

                //Act
                accessor["A"] = 123;

                // Assert
                Assert.AreEqual(123, @struct.A);
            }
        }

        [TestClass]
        public class TheCreateMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullType()
            {
                // Act
                ObjectAccessor.Create(null);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnNonPublicType()
            {
                // Arrange
                var internalClass = new InternalClass();

                // Act
                ObjectAccessor.Create(internalClass);
            }
        }

        [TestClass]
        public class TheGetReadableMemberNamesMethod
        {
            [TestMethod]
            public void ShouldReturnReadableMemberNamesOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = ObjectAccessor.Create(@class);

                // Act
                var memberNames = accessor.GetReadableMemberNames();

                // Assert
                Assert.IsFalse(memberNames
                    .Except(new[] { "A", "AA", "B", "C", "D", "E", "EE", "F", "G", "H", "J" })
                    .Any());
            }

            [TestMethod, ExpectedException(typeof(NotSupportedException))]
            public void ShouldThrowOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = ObjectAccessor.Create(@dynamic);

                // Act
                var memberNames = accessor.GetReadableMemberNames();
            }
        }

        [TestClass]
        public class TheGetWriteableMemberNamesMethod
        {
            [TestMethod]
            public void ShouldReturnWriteableMemberNamesOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = ObjectAccessor.Create(@class);

                // Act
                var memberNames = accessor.GetWriteableMemberNames();

                // Assert
                Assert.IsFalse(memberNames
                    .Except(new[] { "A", "AA", "B", "C", "D", "E", "EE", "F", "G", "H", "K" })
                    .Any());
            }

            [TestMethod, ExpectedException(typeof(NotSupportedException))]
            public void ShouldThrowOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = ObjectAccessor.Create(@dynamic);

                // Act
                var memberNames = accessor.GetWriteableMemberNames();
            }
        }

        [TestClass]
        public class TheGetMemberNamesSupportedProperty
        {
            [TestMethod]
            public void ShouldReturnTrueOnClass()
            {
                // Arrange
                var @class = new Class();

                var accessor = ObjectAccessor.Create(@class);

                // Assert
                Assert.IsTrue(accessor.GetMemberNamesSupported);
            }

            [TestMethod]
            public void ShouldThrowOnDynamic()
            {
                // Arrange
                dynamic @dynamic = new ExpandoObject();

                var accessor = ObjectAccessor.Create(@dynamic);

                // Assert
                Assert.IsFalse(accessor.GetMemberNamesSupported);
            }
        }
    }
}