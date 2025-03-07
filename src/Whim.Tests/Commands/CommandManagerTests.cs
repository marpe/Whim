using AutoFixture;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Whim.TestUtils;
using Xunit;

namespace Whim.Tests;

public class CommandManagerCustomization : ICustomization
{
	public void Customize(IFixture fixture)
	{
		ICommand command = fixture.Freeze<ICommand>();
		command.Id.Returns("command");
		fixture.Inject(command);
	}
}

public class CommandManagerTests
{
	[Theory, AutoSubstituteData<CommandManagerCustomization>]
	public void AddPluginCommand_Success(ICommand command)
	{
		// Given
		CommandManager commandManager = new();

		// When
		commandManager.AddPluginCommand(command);

		// Then
		Assert.Contains(command, commandManager);
		Assert.Equal(command, commandManager.TryGetCommand(command.Id));
		Assert.Single(commandManager);
	}

	[Theory, AutoSubstituteData<CommandManagerCustomization>]
	public void AddPluginCommand_AlreadyContainsCommand(ICommand command)
	{
		// Given
		CommandManager commandManager = new();

		// When
		commandManager.AddPluginCommand(command);
		Assert.Throws<InvalidOperationException>(() => commandManager.AddPluginCommand(command));

		// Then
		Assert.Contains(command, commandManager);
		Assert.Equal(command, commandManager.TryGetCommand(command.Id));
		Assert.Single(commandManager);
	}

	[Fact]
	public void Add()
	{
		// Given
		CommandManager commandManager = new();

		// When
		commandManager.Add("command", "title", () => { });

		// Then
		ICommand? command = commandManager.TryGetCommand("whim.custom.command");
		Assert.NotNull(command);
		Assert.Equal("whim.custom.command", command!.Id);
		Assert.Equal("title", command.Title);
	}

	[Theory, AutoSubstituteData<CommandManagerCustomization>]
	public void GetEnumerator(ICommand command)
	{
		// Given
		CommandManager commandManager = new();

		// When
		commandManager.AddPluginCommand(command);

		// Then
		List<ICommand> allCommands = commandManager.ToList();
		Assert.Single(allCommands);
		Assert.Equal(command, allCommands[0]);
	}
}
