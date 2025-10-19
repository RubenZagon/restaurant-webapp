interface AllergenIconProps {
  allergen: string
  size?: number
}

export function AllergenIcon({ allergen, size = 24 }: AllergenIconProps) {
  const iconColor = '#8B4513' // Brown/Canarian earth color

  const getIcon = () => {
    switch (allergen.toLowerCase()) {
      case 'gluten':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Gluten</title>
            <path d="M12 2L4 7v10l8 5 8-5V7l-8-5z" stroke={iconColor} strokeWidth="2" fill="#F4E4C1"/>
            <path d="M12 8v8M8 10l8 4M8 14l8-4" stroke={iconColor} strokeWidth="1.5"/>
          </svg>
        )
      case 'dairy':
      case 'milk':
      case 'lactose':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Dairy</title>
            <path d="M8 2h8v4H8z" fill={iconColor}/>
            <path d="M7 6h10v14a2 2 0 01-2 2H9a2 2 0 01-2-2V6z" stroke={iconColor} strokeWidth="2" fill="#FFF9E6"/>
            <circle cx="12" cy="14" r="1.5" fill={iconColor}/>
          </svg>
        )
      case 'eggs':
      case 'egg':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Eggs</title>
            <ellipse cx="12" cy="13" rx="6" ry="8" stroke={iconColor} strokeWidth="2" fill="#FFFACD"/>
          </svg>
        )
      case 'fish':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Fish</title>
            <path d="M4 12h12l4-4v8l-4-4H4l2-2-2-2z" stroke={iconColor} strokeWidth="2" fill="#B0E0E6"/>
            <circle cx="14" cy="11" r="1" fill={iconColor}/>
          </svg>
        )
      case 'shellfish':
      case 'crustaceans':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Shellfish</title>
            <path d="M12 4c-3 0-6 2-6 6 0 3 2 6 6 8 4-2 6-5 6-8 0-4-3-6-6-6z" stroke={iconColor} strokeWidth="2" fill="#FFB6C1"/>
            <path d="M8 10l4-6 4 6" stroke={iconColor} strokeWidth="1.5"/>
          </svg>
        )
      case 'nuts':
      case 'tree nuts':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Nuts</title>
            <ellipse cx="12" cy="14" rx="5" ry="7" stroke={iconColor} strokeWidth="2" fill="#DEB887"/>
            <path d="M12 7v4" stroke={iconColor} strokeWidth="2"/>
          </svg>
        )
      case 'peanuts':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Peanuts</title>
            <path d="M9 8c-2 0-3 1.5-3 3.5S7 15 9 15c1 0 1.5-.5 2-1 .5.5 1 1 2 1 2 0 3-1.5 3-3.5S15 8 13 8c-1 0-1.5.5-2 1-.5-.5-1-1-2-1z" stroke={iconColor} strokeWidth="2" fill="#F5DEB3"/>
          </svg>
        )
      case 'soy':
      case 'soya':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Soy</title>
            <circle cx="10" cy="12" r="3" stroke={iconColor} strokeWidth="2" fill="#E6E6CA"/>
            <circle cx="16" cy="12" r="3" stroke={iconColor} strokeWidth="2" fill="#E6E6CA"/>
            <path d="M10 9c0-2 2-4 4-4s4 2 4 4" stroke={iconColor} strokeWidth="1.5"/>
          </svg>
        )
      case 'celery':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Celery</title>
            <rect x="10" y="8" width="4" height="12" rx="1" stroke={iconColor} strokeWidth="2" fill="#90EE90"/>
            <path d="M8 8h8M8 11h8M8 14h8" stroke={iconColor} strokeWidth="1"/>
          </svg>
        )
      case 'mustard':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Mustard</title>
            <circle cx="12" cy="12" r="6" stroke={iconColor} strokeWidth="2" fill="#FFD700"/>
            <circle cx="10" cy="11" r="1" fill={iconColor}/>
            <circle cx="14" cy="11" r="1" fill={iconColor}/>
            <circle cx="12" cy="14" r="1" fill={iconColor}/>
          </svg>
        )
      case 'sesame':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Sesame</title>
            <circle cx="9" cy="10" r="1.5" fill={iconColor}/>
            <circle cx="15" cy="10" r="1.5" fill={iconColor}/>
            <circle cx="12" cy="13" r="1.5" fill={iconColor}/>
            <circle cx="9" cy="16" r="1.5" fill={iconColor}/>
            <circle cx="15" cy="16" r="1.5" fill={iconColor}/>
          </svg>
        )
      case 'sulfites':
      case 'sulphites':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Sulfites</title>
            <circle cx="12" cy="12" r="7" stroke={iconColor} strokeWidth="2" fill="#E6E6FA"/>
            <path d="M12 8v8M8 12h8" stroke={iconColor} strokeWidth="2"/>
          </svg>
        )
      case 'lupin':
      case 'lupine':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Lupin</title>
            <path d="M12 4l-3 6h6l-3-6zM9 10l-3 6h12l-3-6H9z" stroke={iconColor} strokeWidth="2" fill="#DDA0DD"/>
          </svg>
        )
      case 'molluscs':
      case 'mollusks':
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Molluscs</title>
            <path d="M6 12c0-4 3-8 6-8s6 4 6 8c0 3-2 6-6 8-4-2-6-5-6-8z" stroke={iconColor} strokeWidth="2" fill="#F0E68C"/>
            <path d="M9 10c1-1 3-1 6 0" stroke={iconColor} strokeWidth="1.5"/>
          </svg>
        )
      default:
        return (
          <svg width={size} height={size} viewBox="0 0 24 24" fill="none">
            <title>Allergen</title>
            <circle cx="12" cy="12" r="8" stroke={iconColor} strokeWidth="2" fill="#FFE4E1"/>
            <path d="M12 8v4M12 16h.01" stroke={iconColor} strokeWidth="2" strokeLinecap="round"/>
          </svg>
        )
    }
  }

  return <div style={{ display: 'inline-block' }}>{getIcon()}</div>
}

interface AllergenListProps {
  allergens: string[]
  size?: number
  showLabels?: boolean
}

export function AllergenList({ allergens, size = 24, showLabels = false }: AllergenListProps) {
  if (!allergens || allergens.length === 0) return null

  return (
    <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap', alignItems: 'center' }}>
      {allergens.map((allergen) => (
        <div
          key={allergen}
          style={{
            display: 'flex',
            alignItems: 'center',
            gap: '4px',
            padding: showLabels ? '4px 8px' : '4px',
            backgroundColor: '#FFF9E6',
            borderRadius: '6px',
            border: '1px solid #E8D5B7'
          }}
          title={allergen}
        >
          <AllergenIcon allergen={allergen} size={size} />
          {showLabels && (
            <span style={{ fontSize: '12px', color: '#8B4513', fontWeight: '500' }}>
              {allergen}
            </span>
          )}
        </div>
      ))}
    </div>
  )
}

export function AllergenLegend() {
  const commonAllergens = [
    'Gluten', 'Dairy', 'Eggs', 'Fish', 'Shellfish', 'Nuts',
    'Peanuts', 'Soy', 'Celery', 'Mustard', 'Sesame', 'Sulfites'
  ]

  return (
    <div style={{
      backgroundColor: '#FFFBF0',
      border: '2px solid #D4A574',
      borderRadius: '12px',
      padding: '20px',
      marginTop: '20px'
    }}>
      <h3 style={{
        margin: '0 0 16px 0',
        color: '#8B4513',
        fontSize: '18px',
        fontWeight: '600'
      }}>
        Allergen Legend
      </h3>
      <div style={{
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fill, minmax(150px, 1fr))',
        gap: '12px'
      }}>
        {commonAllergens.map((allergen) => (
          <div
            key={allergen}
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              padding: '8px',
              backgroundColor: '#FFF',
              borderRadius: '6px',
              border: '1px solid #E8D5B7'
            }}
          >
            <AllergenIcon allergen={allergen} size={20} />
            <span style={{ fontSize: '13px', color: '#5D4037', fontWeight: '500' }}>
              {allergen}
            </span>
          </div>
        ))}
      </div>
    </div>
  )
}
