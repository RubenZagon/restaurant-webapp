import { Category } from '@infrastructure/api/productsApi'

interface CategoryTabsProps {
  categories: Category[]
  selectedCategoryId: string | null
  onSelectCategory: (categoryId: string) => void
}

function CategoryTabs({ categories, selectedCategoryId, onSelectCategory }: CategoryTabsProps) {
  return (
    <div style={{
      display: 'flex',
      gap: '8px',
      overflowX: 'auto',
      padding: '8px 0',
      marginBottom: '24px',
      borderBottom: '2px solid #e0e0e0'
    }}>
      {categories.map((category) => (
        <button
          key={category.id}
          onClick={() => onSelectCategory(category.id)}
          style={{
            padding: '12px 24px',
            border: 'none',
            borderRadius: '8px 8px 0 0',
            backgroundColor: selectedCategoryId === category.id ? '#3498db' : '#ecf0f1',
            color: selectedCategoryId === category.id ? '#fff' : '#2c3e50',
            cursor: 'pointer',
            fontWeight: selectedCategoryId === category.id ? 'bold' : 'normal',
            transition: 'all 0.3s',
            whiteSpace: 'nowrap'
          }}
        >
          {category.name}
        </button>
      ))}
    </div>
  )
}

export default CategoryTabs
