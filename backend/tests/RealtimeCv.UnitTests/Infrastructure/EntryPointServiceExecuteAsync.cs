﻿// using System;
// using System.Threading.Tasks;
// using RealtimeCv.Infrastructure.Messaging;
// using Xunit;
//
// namespace CleanArchitecture.UnitTests;
//
// public class InMemoryQueueReceiverGetMessageFromQueue
// {
//     private readonly InMemoryQueueReceiver _receiver = new InMemoryQueueReceiver();
//     [Fact]
//     public async Task ThrowsNullExceptionGivenNullQueuename()
//     {
//         var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _receiver.GetMessageFromQueue(null));
//     }
//
//     [Fact]
//     public async Task ThrowsArgumentExceptionGivenEmptyQueuename()
//     {
//         var ex = await Assert.ThrowsAsync<ArgumentException>(() => _receiver.GetMessageFromQueue(string.Empty));
//     }
// }
