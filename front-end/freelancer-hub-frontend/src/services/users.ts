const apiUrl = import.meta.env.VITE_API_URL

export async function fetchUsers() {
  const res = await fetch(`${apiUrl}/User`)
  if (!res.ok) {
    throw new Error(`Erro: ${res.status}`)
  }
  return res.json()
}
