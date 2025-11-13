// https://raw.githubusercontent.com/neosmart/AsyncLock/refs/heads/master/AsyncLock/AsyncLock.cs

using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace Sb.Extensions.System.Threading;

/// <summary>
/// </summary>
public sealed class AsyncLock
{
  internal const long UnlockedId = 0x00; // "owning" task id when unlocked

  private static long _asyncStackCounter;

  // An AsyncLocal<T> is not really the task-based equivalent to a ThreadLocal<T>, in that
  // it does not track the async flow (as the documentation describes) but rather it is
  // associated with a stack snapshot. Mutation of the AsyncLocal in an await call does
  // not change the value observed by the parent when the call returns, so if you want to
  // use it as a persistent async flow identifier, the value needs to be set at the outer-
  // most level and never touched internally.

  private static readonly AsyncLocal<long> _asyncId = new();
  internal readonly SemaphoreSlim Reentrancy = new(1, 1);

  // We are using this SemaphoreSlim like a posix condition variable.
  // We only want to wake waiters, one or more of whom will try to obtain
  // a different lock to do their thing. So long as we can guarantee no
  // wakes are missed, the number of awakees is not important.
  // Ideally, this would be "friend" for access only from InnerLock, but
  // whatever.
  internal readonly SemaphoreSlim Retry = new(0, 1);
  internal long OwningId = UnlockedId;
  internal int OwningThreadId = (int)UnlockedId;

  internal int Reentrances;

  internal static long AsyncId => _asyncId.Value;
  internal static int ThreadId => Thread.CurrentThread.ManagedThreadId;


  /// <summary>
  ///   Make sure InnerLock.LockAsync() does not use await, because an async function triggers a snapshot of
  ///   the AsyncLocal value.
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
  {
    var @lock = new InnerLock(this, _asyncId.Value, ThreadId);
    _asyncId.Value = Interlocked.Increment(ref _asyncStackCounter);
    return @lock.ObtainLockAsync(cancellationToken);
  }

  /// <summary>
  ///   Make sure InnerLock.LockAsync() does not use await, because an async function triggers a snapshot of
  ///   the AsyncLocal value.
  /// </summary>
  /// <param name="callback"></param>
  /// <param name="timeout"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<bool> TryLockAsync(Action callback, TimeSpan timeout, CancellationToken cancellationToken = default)
  {
    var @lock = new InnerLock(this, _asyncId.Value, ThreadId);
    _asyncId.Value = Interlocked.Increment(ref _asyncStackCounter);
    var disposableLock = await @lock.TryObtainLockAsync(timeout, cancellationToken).ConfigureAwait(false);
    if (disposableLock is null) return false;
    try
    {
      callback();
    }
    finally
    {
      disposableLock.Dispose();
    }

    return true;
  }

  /// <summary>
  ///   Make sure InnerLock.LockAsync() does not use await, because an async function triggers a snapshot of
  ///   the AsyncLocal value.
  /// </summary>
  /// <param name="callback"></param>
  /// <param name="timeout"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<bool> TryLockAsync(Func<Task> callback, TimeSpan timeout,
    CancellationToken cancellationToken = default)
  {
    var @lock = new InnerLock(this, _asyncId.Value, ThreadId);
    _asyncId.Value = Interlocked.Increment(ref _asyncStackCounter);
    var disposableLock = await @lock.TryObtainLockAsync(timeout, cancellationToken).ConfigureAwait(false);
    if (disposableLock is null) return false;
    try
    {
      await callback().ConfigureAwait(false);
    }
    finally
    {
      disposableLock.Dispose();
    }

    return true;
  }

  /// <summary>
  ///   Make sure InnerLock.TryLockAsync() does not use await, because an async function triggers a snapshot of
  ///   the AsyncLocal value.
  /// </summary>
  /// <param name="callback"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<bool> TryLockAsync(Action callback, CancellationToken cancellationToken)
  {
    var @lock = new InnerLock(this, _asyncId.Value, ThreadId);
    _asyncId.Value = Interlocked.Increment(ref _asyncStackCounter);
    var disposableLock = await @lock.TryObtainLockAsync(cancellationToken).ConfigureAwait(false);
    if (disposableLock is null) return false;
    try
    {
      callback();
    }
    finally
    {
      disposableLock.Dispose();
    }

    return true;
  }

  /// <summary>
  ///   Make sure InnerLock.LockAsync() does not use await, because an async function triggers a snapshot of
  ///   the AsyncLocal value.
  /// </summary>
  /// <param name="callback"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<bool> TryLockAsync(Func<Task> callback, CancellationToken cancellationToken)
  {
    var @lock = new InnerLock(this, _asyncId.Value, ThreadId);
    _asyncId.Value = Interlocked.Increment(ref _asyncStackCounter);
    var disposableLock = await @lock.TryObtainLockAsync(cancellationToken).ConfigureAwait(false);
    if (disposableLock is null) return false;
    try
    {
      await callback().ConfigureAwait(false);
    }
    finally
    {
      disposableLock.Dispose();
    }

    return true;
  }

  /// <summary>
  ///   Lock
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public IDisposable Lock(CancellationToken cancellationToken = default)
  {
    var @lock = new InnerLock(this, _asyncId.Value, ThreadId);
    // Increment the async stack counter to prevent a child task from getting
    // the lock at the same time as a child thread.
    _asyncId.Value = Interlocked.Increment(ref _asyncStackCounter);
    return @lock.ObtainLock(cancellationToken);
  }

  /// <summary>
  ///   TryLock
  /// </summary>
  /// <param name="callback"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public bool TryLock(Action callback, TimeSpan timeout)
  {
    var @lock = new InnerLock(this, _asyncId.Value, ThreadId);
    // Increment the async stack counter to prevent a child task from getting
    // the lock at the same time as a child thread.
    _asyncId.Value = Interlocked.Increment(ref _asyncStackCounter);
    var lockDisposable = @lock.TryObtainLock(timeout);
    if (lockDisposable is null) return false;

    // Execute the callback then release the lock
    try
    {
      callback();
    }
    finally
    {
      lockDisposable.Dispose();
    }

    return true;
  }
}

#region InnerLock

/// <summary>
/// </summary>
public readonly struct InnerLock : IDisposable
{
  private readonly AsyncLock _parent;
  private readonly long _oldId;
  private readonly int _oldThreadId;

  internal InnerLock(AsyncLock parent, long oldId, int oldThreadId)
  {
    _parent = parent;
    _oldId = oldId;
    _oldThreadId = oldThreadId;
  }

  internal async Task<IDisposable> ObtainLockAsync(CancellationToken cancellationToken = default)
  {
    while (true)
    {
      await _parent.Reentrancy.WaitAsync(cancellationToken).ConfigureAwait(false);
      if (InnerTryEnter()) break;
      // We need to wait for someone to leave the lock before trying again.
      // We need to "atomically" obtain _retry and release _reentrancy, but there
      // is no equivalent to a condition variable. Instead, we call *but don't await*
      // _retry.WaitAsync(), then release the reentrancy lock, *then* await the saved task.
      var waitTask = _parent.Retry.WaitAsync(cancellationToken).ConfigureAwait(false);
      _parent.Reentrancy.Release();
      await waitTask;
    }

    // Reset the owning thread id after all await calls have finished, otherwise we
    // could be resumed on a different thread and set an incorrect value.
    _parent.OwningThreadId = AsyncLock.ThreadId;
    _parent.Reentrancy.Release();
    return this;
  }

  internal async Task<IDisposable?> TryObtainLockAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
  {
    // In case of zero-timeout, don't even wait for protective lock contention
    if (timeout == TimeSpan.Zero)
    {
      await _parent.Reentrancy.WaitAsync(timeout, cancellationToken);
      if (InnerTryEnter())
      {
        // Reset the owning thread id after all await calls have finished, otherwise we
        // could be resumed on a different thread and set an incorrect value.
        _parent.OwningThreadId = AsyncLock.ThreadId;
        _parent.Reentrancy.Release();
        return this;
      }

      _parent.Reentrancy.Release();
      return null;
    }

    var now = DateTimeOffset.UtcNow;
    var last = now;
    var remainder = timeout;

    // We need to wait for someone to leave the lock before trying again.
    while (remainder > TimeSpan.Zero)
    {
      await _parent.Reentrancy.WaitAsync(remainder, cancellationToken).ConfigureAwait(false);
      if (InnerTryEnter())
      {
        _parent.OwningThreadId = AsyncLock.ThreadId;
        _parent.Reentrancy.Release();
        return this;
      }

      _parent.Reentrancy.Release();

      now = DateTimeOffset.UtcNow;
      remainder -= now - last;
      last = now;
      if (remainder < TimeSpan.Zero)
      {
        _parent.Reentrancy.Release();
        return null;
      }

      var waitTask = _parent.Retry.WaitAsync(remainder, cancellationToken).ConfigureAwait(false);
      _parent.Reentrancy.Release();
      if (!await waitTask) return null;

      now = DateTimeOffset.UtcNow;
      remainder -= now - last;
      last = now;
    }

    return null;
  }

  internal async Task<IDisposable?> TryObtainLockAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      while (true)
      {
        await _parent.Reentrancy.WaitAsync(cancellationToken).ConfigureAwait(false);
        if (InnerTryEnter()) break;
        // We need to wait for someone to leave the lock before trying again.
        var waitTask = _parent.Retry.WaitAsync(cancellationToken).ConfigureAwait(false);
        _parent.Reentrancy.Release();
        await waitTask;
      }
    }
    catch (OperationCanceledException)
    {
      return null;
    }

    // Reset the owning thread id after all await calls have finished, otherwise we
    // could be resumed on a different thread and set an incorrect value.
    _parent.OwningThreadId = AsyncLock.ThreadId;
    _parent.Reentrancy.Release();
    return this;
  }

  internal IDisposable ObtainLock(CancellationToken cancellationToken = default)
  {
    while (true)
    {
      _parent.Reentrancy.Wait(cancellationToken);
      if (InnerTryEnter(true))
      {
        _parent.Reentrancy.Release();
        break;
      }

      // We need to wait for someone to leave the lock before trying again.
      var waitTask = _parent.Retry.WaitAsync(cancellationToken);
      _parent.Reentrancy.Release();
      // This should be safe since the task we are awaiting doesn't need to make progress
      // itself to complete - it will be completed by another thread altogether. cf SemaphoreSlim internals.
      waitTask.GetAwaiter().GetResult();
    }

    return this;
  }

  internal IDisposable? TryObtainLock(TimeSpan timeout)
  {
    // In case of zero-timeout, don't even wait for protective lock contention
    if (timeout == TimeSpan.Zero)
    {
      _parent.Reentrancy.Wait(timeout);
      if (InnerTryEnter(true))
      {
        _parent.Reentrancy.Release();
        return this;
      }

      _parent.Reentrancy.Release();
      return null;
    }

    var now = DateTimeOffset.UtcNow;
    var last = now;
    var remainder = timeout;

    // We need to wait for someone to leave the lock before trying again.
    while (remainder > TimeSpan.Zero)
    {
      _parent.Reentrancy.Wait(remainder);
      if (InnerTryEnter(true))
      {
        _parent.Reentrancy.Release();
        return this;
      }

      now = DateTimeOffset.UtcNow;
      remainder -= now - last;
      last = now;

      var waitTask = _parent.Retry.WaitAsync(remainder);
      _parent.Reentrancy.Release();
      if (!waitTask.GetAwaiter().GetResult()) return null;

      now = DateTimeOffset.UtcNow;
      remainder -= now - last;
      last = now;
    }

    return null;
  }

  private bool InnerTryEnter(bool synchronous = false)
  {
    if (synchronous)
    {
      if (_parent.OwningThreadId == AsyncLock.UnlockedId)
        _parent.OwningThreadId = AsyncLock.ThreadId;
      else if (_parent.OwningThreadId != AsyncLock.ThreadId) return false;
      _parent.OwningId = AsyncLock.AsyncId;
    }
    else
    {
      if (_parent.OwningId == AsyncLock.UnlockedId)
        _parent.OwningId = AsyncLock.AsyncId;
      else if (_parent.OwningId != _oldId)
        // Another thread currently owns the lock
        return false;
      else
        // Nested re-entrance
        _parent.OwningId = AsyncLock.AsyncId;
    }

    // We can go in
    _parent.Reentrances += 1;
    return true;
  }

  /// <inheritdoc />
  public void Dispose()
  {
    var @this = this;
    var oldId = _oldId;
    var oldThreadId = _oldThreadId;
    @this._parent.Reentrancy.Wait();
    try
    {
      @this._parent.Reentrances -= 1;
      @this._parent.OwningId = oldId;
      @this._parent.OwningThreadId = oldThreadId;
      if (@this._parent.Reentrances == 0)
      {
        // The owning thread is always the same so long as we
        // are in a nested stack call. We reset the owning id
        // only when the lock is fully unlocked.
        @this._parent.OwningId = AsyncLock.UnlockedId;
        @this._parent.OwningThreadId = (int)AsyncLock.UnlockedId;
      }

      // We can't place this within the _reentrances == 0 block above because we might
      // still need to notify a parallel reentrant task to wake. I think.
      // This should not be a race condition since we only wait on _retry with _reentrancy locked,
      // then release _reentrancy so the Dispose() call can obtain it to signal _retry in a big hack.
      if (@this._parent.Retry.CurrentCount == 0) @this._parent.Retry.Release();
    }
    finally
    {
      @this._parent.Reentrancy.Release();
    }
  }

  #endregion
}