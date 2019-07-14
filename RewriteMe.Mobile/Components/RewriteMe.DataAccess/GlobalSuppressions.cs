// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.DataAccess.Entities.FileItemEntity.TranscribeItems")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.DataAccess.Entities.TranscriptAudioSourceEntity.Source")]
[assembly: SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.DataAccess.IAppDbContext.InsertOrReplaceAsync(System.Object)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.DataAccess.Entities.RecordedItemEntity.AudioFiles")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.DataAccess.Entities.RecordedAudioFileEntity.Source")]