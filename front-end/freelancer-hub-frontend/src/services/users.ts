const apiUrl = import.meta.env.VITE_API_URL

export const fetchUsers = async (token?: string) => {
  try {
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    }
    
    // Adicionar token se fornecido
    if (token) {
      headers.Authorization = `Bearer ${token}`
    }

    headers.accept = 'accept: text/plain'
    
    const response = await fetch(apiUrl + '/Client', {
      method: 'GET',
      headers
    })
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    return await response.json()
  } catch (error) {
    console.error('Erro ao buscar usuários:', error)
    throw error
  }
}