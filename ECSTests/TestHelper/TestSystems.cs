using ECS;
using ECS.Entities;
using ECS.Matching;
using ECS.Systems;
using ECS.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTests.TestHelper
{
    public class TestReactiveSystem : ReactiveSystem
    {
        public List<Entity> UpdatedEntities { get; private set; }
        public bool DidExecute { get; private set; }

        public TestReactiveSystem(Scene scene) : base(scene)
        {
            UpdatedEntities = new List<Entity>();
        }

        protected override Matcher GetMatcher() => new Matcher().Of<TestComponent1>();

        public override void Execute(IEnumerable<Entity> entities)
        {
            DidExecute = true;
            foreach(var e in entities)
            {
                UpdatedEntities.Add(e);
            }
        }
    }

    public class TestInitSystem: IInitializeSystem
    {
        public bool DidInit { get; private set; }

        public void Initialize()
        {
            DidInit = true;
        }
    }

    public class TestExecuteSystem: IExecuteSystem
    {
        public bool DidExecute { get; private set; }
        public int RunCount { get; private set; }

        public void Execute()
        {
            DidExecute = true;
            RunCount++;
        }
    }

    public class TestExecuteInitSystem: IExecuteSystem, IInitializeSystem
    {
        public bool DidInit { get; private set; }
        public bool DidExecute { get; private set; }

        public void Execute()
        {
            DidExecute = true;
        }

        public void Initialize()
        {
            DidInit = true;
        }
    }

    public class TestThreadedReactiveSystem : ThreadSafeReactiveSystem
    {
        public TestThreadedReactiveSystem(Scene scene) : base(scene)
        {
        }

        protected override Matcher GetMatcher() => new Matcher().Of<TestComponent1>();

        public override void Execute(Entity entity)
        {
            entity.UpdateComponent((TestComponent1 comp) =>
            {
                comp.X++;
                comp.Y++;
            });
        }
    }

    public class TestGroupExecutePrintSystem : GroupExecuteSystem
    {
        public TestGroupExecutePrintSystem(Scene scene) : base(scene)
        {
        }

        public override Matcher GetMatcher() => new Matcher().Of<TestComponent1>();

        public override void Execute(Entity entity)
        {
            entity.UpdateComponent((TestComponent1 comp) =>
            {
                Debug.WriteLine($"{comp.X}, {comp.Y}");
            });
        }
    }

    public class TestGroupExecuteMoveSystem : GroupExecuteSystem
    {
        public TestGroupExecuteMoveSystem(Scene scene) : base(scene)
        {
        }

        public override Matcher GetMatcher() => new Matcher().Of<TestComponent1>();

        public override void Execute(Entity entity)
        {
            entity.UpdateComponent((TestComponent1 comp) =>
            {
                comp.X++;
                comp.Y++;
            });
        }
    }
}
