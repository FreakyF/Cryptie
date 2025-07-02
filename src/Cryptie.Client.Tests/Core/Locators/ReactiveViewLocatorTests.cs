using System;
using System.Reflection;
using ReactiveUI;
using Xunit;
using Cryptie.Client.Core.Locators;

namespace Cryptie.Client.Tests.Core.Locators
{
    public class ReactiveViewLocatorTests
    {
        private readonly ReactiveViewLocator _locator = new ReactiveViewLocator();

        [Fact]
        public void ResolveView_NullViewModel_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _locator.ResolveView<object>(null));
        }

        [Fact]
        public void ResolveView_NoViewFound_ThrowsInvalidOperationException()
        {
            var vm = new MissingViewModel();
            Assert.Throws<InvalidOperationException>(() => _locator.ResolveView(vm));
        }

        [Fact]
        public void ResolveView_ViewNotImplementingIViewFor_ThrowsInvalidOperationException()
        {
            var vm = new NoInterfaceViewModel();
            Assert.Throws<InvalidOperationException>(() => _locator.ResolveView(vm));
        }

        [Fact]
        public void ResolveView_WithValidView_ReturnsViewWithViewModel()
        {
            var vm = new DummyViewModel();
            var view = _locator.ResolveView(vm);
            Assert.NotNull(view);
            Assert.IsType<DummyView>(view);
            Assert.Equal(vm, view.ViewModel);
        }

        [Fact]
        public void FindViewType_NullFullName_ReturnsNull()
        {
            // Use reflection to invoke private FindViewType
            var method = typeof(ReactiveViewLocator)
                .GetMethod("FindViewType", BindingFlags.NonPublic | BindingFlags.Static);
            var fakeType = new FakeType();
            var result = method.Invoke(null, new object[] { fakeType });
            Assert.Null(result);
        }

        // Dummy types for tests
        public class DummyViewModel { }
        public class DummyView : IViewFor<DummyViewModel>
        {
            public DummyViewModel ViewModel { get; set; }
            object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (DummyViewModel)value; }
        }

        public class NoInterfaceViewModel { }
        public class NoInterfaceView { }

        public class MissingViewModel { }

        private class FakeType : Type
        {
            public override string FullName => null;
            public override string AssemblyQualifiedName => throw new NotImplementedException();
            public override Assembly Assembly => throw new NotImplementedException();
            public override string Namespace => throw new NotImplementedException();
            public override Type BaseType => throw new NotImplementedException();
            public override Type UnderlyingSystemType => throw new NotImplementedException();
            public override string Name => throw new NotImplementedException();
            public override Module Module => throw new NotImplementedException();
            public override Guid GUID => throw new NotImplementedException();
            public override int MetadataToken => throw new NotImplementedException();
            public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => throw new NotImplementedException();
            public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();
            public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException();
            public override Type GetElementType() => throw new NotImplementedException();
            public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
            public override EventInfo[] GetEvents(BindingFlags bindingAttr) => throw new NotImplementedException();
            public override FieldInfo GetField(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
            public override FieldInfo[] GetFields(BindingFlags bindingAttr) => throw new NotImplementedException();
            public override Type GetInterface(string name, bool ignoreCase) => throw new NotImplementedException();
            public override Type[] GetInterfaces() => throw new NotImplementedException();
            public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => throw new NotImplementedException();
            public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => throw new NotImplementedException();
            public override Type[] GetNestedTypes(BindingFlags bindingAttr) => throw new NotImplementedException();
            public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => throw new NotImplementedException();
            public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) => throw new NotImplementedException();
            public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();
            protected override TypeAttributes GetAttributeFlagsImpl() => throw new NotImplementedException();
            protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();
            protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();
            protected override bool HasElementTypeImpl() => throw new NotImplementedException();
            protected override bool IsArrayImpl() => throw new NotImplementedException();
            protected override bool IsCOMObjectImpl() => throw new NotImplementedException();
            protected override bool IsPointerImpl() => throw new NotImplementedException();
            protected override bool IsPrimitiveImpl() => throw new NotImplementedException();
            public override Type GetNestedType(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
            protected override bool IsByRefImpl() => throw new NotImplementedException();
            protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();
        }
    }
}
