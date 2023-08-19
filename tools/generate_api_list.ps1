roslynator list-symbols generate_ref_docs.sln `
 --properties Configuration=Release `
 --visibility public `
 --depth member `
 --ignored-parts containing-namespace assembly-attributes `
 --output "api.txt"
