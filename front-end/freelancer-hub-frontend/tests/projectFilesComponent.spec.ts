import { mount } from '@vue/test-utils'
import { describe, it, expect } from 'vitest'
import FilesSection from '@/components/ProjectFiles.vue'

describe('FilesSection.vue', () => {
  const mockFiles = [
    {
      id: '1',
      fileName: 'Relatorio',
      fileExtension: '.pdf',
      fileSize: 2048,
      createdAt: new Date('2025-01-10').toISOString(),
    },
  ]

  it('renderiza mensagem de "Nenhum arquivo" quando não há arquivos', () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: [],
        isLoadingFiles: false,
        fileError: '',
      },
    })

    expect(wrapper.find('.no-files').exists()).toBe(true)
    expect(wrapper.text()).toContain('Nenhum arquivo adicionado ao projeto')
  })

  it('mostra indicador de carregamento quando isLoadingFiles é true', () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: [],
        isLoadingFiles: true,
        fileError: '',
      },
    })

    expect(wrapper.find('.loading-files').exists()).toBe(true)
    expect(wrapper.text()).toContain('Carregando arquivos...')
  })

  it('mostra mensagem de erro quando fileError é fornecido', () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: [],
        isLoadingFiles: false,
        fileError: 'Erro ao carregar arquivo',
      },
    })

    expect(wrapper.find('.file-error').exists()).toBe(true)
    expect(wrapper.text()).toContain('Erro ao carregar arquivo')
  })

  it('renderiza corretamente lista de arquivos', () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: mockFiles,
        isLoadingFiles: false,
        fileError: '',
      },
    })

    expect(wrapper.findAll('.file-card')).toHaveLength(1)
    expect(wrapper.text()).toContain('Relatorio.pdf')
    expect(wrapper.text()).toContain('2 KB')
    expect(wrapper.find('.fa-file-pdf').exists()).toBe(true)
  })

  it('emite evento "file-upload" ao selecionar arquivo', async () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: [],
        isLoadingFiles: false,
        fileError: '',
      },
    })

    const input = wrapper.find('input[type="file"]').element as HTMLInputElement

    // Cria um arquivo falso e evento real
    const mockFile = new File(['conteúdo'], 'teste.txt', { type: 'text/plain' })
    const event = new Event('change')
    Object.defineProperty(event, 'target', {
      writable: false,
      value: { files: [mockFile] },
    })

    input.dispatchEvent(event)
    await wrapper.vm.$nextTick()

    expect(wrapper.emitted('file-upload')).toBeTruthy()
    const emittedEvent = wrapper.emitted('file-upload')![0][0]
    expect(emittedEvent).toBeInstanceOf(Event)
    expect(emittedEvent.target.files[0].name).toBe('teste.txt')
  })

  it('emite evento "download-file" ao clicar em download', async () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: mockFiles,
        isLoadingFiles: false,
        fileError: '',
      },
    })

    const button = wrapper.find('.download-button')
    await button.trigger('click')

    expect(wrapper.emitted('download-file')).toBeTruthy()
    expect(wrapper.emitted('download-file')![0][0]).toEqual(mockFiles[0])
  })

  it('emite evento "delete-file" ao clicar em excluir', async () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: mockFiles,
        isLoadingFiles: false,
        fileError: '',
      },
    })

    const button = wrapper.find('.delete-button')
    await button.trigger('click')

    expect(wrapper.emitted('delete-file')).toBeTruthy()
    expect(wrapper.emitted('delete-file')![0][0]).toBe('1')
  })

  it('formata corretamente o tamanho do arquivo', () => {
    const wrapper = mount(FilesSection, {
      props: {
        files: mockFiles,
        isLoadingFiles: false,
        fileError: '',
      },
    })

    expect(wrapper.text()).toContain('2 KB')
  })

  it('usa ícone genérico quando extensão não é reconhecida', () => {
    const files = [
      { id: '2', fileName: 'Arquivo', fileExtension: '.xyz', fileSize: 100, createdAt: new Date().toISOString() },
    ]
    const wrapper = mount(FilesSection, {
      props: {
        files,
        isLoadingFiles: false,
        fileError: '',
      },
    })

    expect(wrapper.find('.fa-file').exists()).toBe(true)
  })
})
