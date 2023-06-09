using System;
using CodeName.EventSystem.Tasks;
using NUnit.Framework;

namespace CodeName.EventSystem.Tests
{
    [TestFixture]
    public class StateTaskTests
    {
        private const int Result = 5;

        [Test]
        public void StateTask_WhenCompletionSourceIsCompleted_IsCompleted()
        {
            var source = new StateTaskCompletionSource();
            var task = new StateTask(source);

            Assert.IsFalse(task.IsCompleted);
            source.Complete();
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void StateTask_CompletedTask_IsCompleted()
        {
            Assert.IsTrue(StateTask.CompletedTask.IsCompleted);
            Assert.IsTrue(StateTask<int>.CompletedTask.IsCompleted);
        }

        [Test]
        public void StateTask_OnException_RethrowsException()
        {
            async StateTask Run()
            {
                await StateTask.CompletedTask;

                throw new InvalidCastException();
            }

            Assert.Throws<InvalidCastException>(() =>
            {
                Run().GetAwaiter().GetResult();
            });
        }

        [Test]
        public void StateTask_ReturnsCorrectResult()
        {
            async StateTask<int> Run()
            {
                await StateTask.CompletedTask;

                return Result;
            }

            Assert.AreEqual(Result, Run().GetAwaiter().GetResult());
        }

        [Test]
        public void StateTask_FromResult_ReturnsCompletedTaskWithResult()
        {
            var task = StateTask<int>.FromResult(Result);

            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(Result, task.GetAwaiter().GetResult());
        }

        [Test]
        public void StateTask_WhenConvertedToNonGeneric_CompletesAtSameTimeAsOriginal()
        {
            var source = new StateTaskCompletionSource<int>();
            var originalTask = new StateTask<int>(source);
            var convertedTask = originalTask.ToStateTask();

            Assert.IsFalse(originalTask.IsCompleted);
            Assert.IsFalse(convertedTask.IsCompleted);

            source.Complete(Result);

            Assert.IsTrue(originalTask.IsCompleted);
            Assert.IsTrue(convertedTask.IsCompleted);

            Assert.AreEqual(Result, originalTask.GetAwaiter().GetResult());
        }

        [Test]
        public void StateTask_PropagatesExceptionBackToOriginalCaller()
        {
            async StateTask RunRecursive(int counter)
            {
                if (counter < 0)
                {
                    throw new InvalidCastException();
                }

                await RunRecursive(counter - 1);
            }

            async StateTask<int> RunRecursiveWithResult(int counter)
            {
                if (counter < 0)
                {
                    throw new InvalidCastException();
                }

                return await RunRecursiveWithResult(counter - 1);
            }

            Assert.Throws<InvalidCastException>(() =>
            {
                RunRecursive(20).GetAwaiter().GetResult();
            });

            Assert.Throws<InvalidCastException>(() =>
            {
                RunRecursiveWithResult(20).GetAwaiter().GetResult();
            });
        }

        [Test]
        public void StateTask_PropagatesCompletionBackToOriginalCaller()
        {
            async StateTask RunRecursive(int counter)
            {
                if (counter < 0)
                {
                    return;
                }

                await RunRecursive(counter - 1);
            }

            async StateTask<int> RunRecursiveWithResult(int counter)
            {
                if (counter < 0)
                {
                    return Result;
                }

                return await RunRecursiveWithResult(counter - 1);
            }

            Assert.DoesNotThrow(() =>
            {
                RunRecursive(20).GetAwaiter().GetResult();
            });

            Assert.DoesNotThrow(() =>
            {
                var result = RunRecursiveWithResult(20).GetAwaiter().GetResult();

                Assert.AreEqual(Result, result);
            });
        }

        [Test]
        public void StateTask_GettingResultWhenIsNotComplete_Throws()
        {
            var source = new StateTaskCompletionSource();
            var task = new StateTask(source);

            var genericSource = new StateTaskCompletionSource<int>();
            var genericTask = new StateTask<int>(genericSource);

            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(genericTask.IsCompleted);

            Assert.Throws<InvalidOperationException>(() =>
            {
                task.GetAwaiter().GetResult();
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                genericTask.GetAwaiter().GetResult();
            });
        }

        [Test]
        public void StateTask_GettingResultWhenComplete_DoesNotThrow()
        {
            var source = new StateTaskCompletionSource();
            var task = new StateTask(source);

            var genericSource = new StateTaskCompletionSource<int>();
            var genericTask = new StateTask<int>(genericSource);

            source.Complete();
            genericSource.Complete(Result);

            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(genericTask.IsCompleted);

            Assert.DoesNotThrow(() =>
            {
                task.GetAwaiter().GetResult();
            });

            Assert.DoesNotThrow(() =>
            {
                genericTask.GetAwaiter().GetResult();
            });
        }

        [Test]
        public void StateTask_WhenCompletionSourceNeverCompleted_NeverCompletes()
        {
            StateTask DoesNotComplete()
            {
                return new StateTask(new StateTaskCompletionSource());
            }

            async StateTask Run()
            {
                await StateTask.CompletedTask;

                await DoesNotComplete();
            }

            StateTask<int> DoesNotCompleteGeneric()
            {
                return new StateTask<int>(new StateTaskCompletionSource<int>());
            }

            async StateTask<int> RunGeneric()
            {
                await StateTask<int>.CompletedTask;

                await DoesNotCompleteGeneric();

                throw new InvalidCastException();
            }

            Assert.Throws<InvalidOperationException>(() =>
            {
                Run().GetAwaiter().GetResult();
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                RunGeneric().GetAwaiter().GetResult();
            });
        }
    }
}
